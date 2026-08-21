using loremiscExpansion.CWTs;
using MoreSlugcats;
using RWCustom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Watcher;
using RWCustom;
using static loremiscExpansion.Plugin;

namespace loremiscExpansion.Conduit
{
    public static class ConduitHooks
    {
        public static int armorChunk = 1;

        public static int maxBreathTime = 80;

        public static float throwboostVelocity = 12f;

        public static float maxSporepuffPoison = 1f;
        public static float sporepuffPoisonSpeed = 0.01f;
        public static float sporepuffPoisonRecovery = 0.003f;

        public static int maxDodgeRollWindow = 10;
        public static bool IsConduit(this Player self)
        {
            Log.LogMessage("Checking if Player is Conduit!");
            bool result = self != null && self.SlugCatClass == Enums.protag;
            Log.LogMessage(result);
            return result;
        }

        public static bool IsCrouched(this Player self)
        {
            return self?.bodyMode == Player.BodyModeIndex.Crawl;
            // TODO
        }

        public static bool IsRolling(this Player self)
        {
            if (!IsConduit(self)) return false;
            if (!PlayerCWT.TryGetData(self, out var data)) return false;
            return data.rolling;
        }

        public static void SetRolling(this Player self, bool value, int duration = -1)
        {
            if (!CWTs.PlayerCWT.TryGetData(self, out var data)) return;
            data.rolling = value;
            data.rollCooldown = 20;
            if (!value) return;
            data.rollAnimation = 20;
            self.animation = Player.AnimationIndex.Roll;
            if (duration > 0) data.temporaryRollDuration = duration;
            self.room.PlaySound(SoundID.Slugcat_Roll_Init, self.mainBodyChunk, loop: false, 1f, 1f);
        }

        public static void ApplyHooks()
        {
            On.Player.AllowGrabbingBatflys += Player_AllowGrabbingBatflys; // Protag doesnt grab batflies automatically
            On.Player.Blink += Player_Blink; // Protag cannot blink
            On.Player.CanBeGrabbed += Player_CanBeGrabbed; // Protag cannot grab slugpups (they are irresponsible)
            On.Player.CanBeSwallowed += Player_CanBeSwallowed; // Protag cannot swallow items
            On.Player.CanEatMeat += Player_CanEatMeat; // TODO: Change Protags diet
            On.Player.CanIPickThisUp += Player_CanIPickThisUp; // Protag cannot grab slugpups (they are irresponsible)
            On.Player.Die += Player_Die; // Protag stops rolling and emitting air when dying
            On.Player.Jump += Player_Jump; // Cricket jump
            On.Player.JumpOnChunk += Player_JumpOnChunk; // Roll bounce off creatures
            On.Player.LungUpdate += Lung_Update; // Protag has breath reserves they can use
            On.Player.MovementUpdate += Player_MovementUpdate; // TODO
            On.Player.SpearStick += Player_SpearStick; // Spears bounce off Protag if they are rolling
            On.Player.Stun += Player_Stun; // Protag stops rolling and gets the current air reserve interruped when stunned
            On.Player.SwallowObject += Player_SwallowObject; // Protag cannot swallow items
            On.Player.TerrainImpact += Player_TerrainImpact; // Roll bounce off ground, TODO: Roll bounce off walls
            On.Player.ThrownSpear += Player_ThrownSpear; // Protag can throw spears underwater
            On.Player.ThrowObject += Player_ThrowObject; // Protag can throw items in any direction regardless of state
            On.Player.Update += Player_Update; // Everything
            On.Player.UpdateBodyMode += Player_UpdateBodyMode; // TODO: Remove slides and replace with dodge rolls

            // TODO: Camouflage


        }

        public static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            orig(self);
        }

        public static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            if (!IsConduit(self))  return;
            if (!PlayerCWT.TryGetData(self, out var data)) return;
            if (data.dodgeRollWindow > 0)
            {
                data.dodgeRollWindow--;
            }
            self.eyesClosedTime = 0;
            self.blind = 0;
            if (self.State is PlayerState state && state.permanentDamageTracking > 0) state.permanentDamageTracking -= 0.0002;
            self.longBellySlide = false;
            self.superLaunchJump = 0;
            if (self.sporeParticleTicks > 0)
            {
                data.sporePoison += sporepuffPoisonSpeed;
                if (data.sporePoison > 0.2f && Random.value < 0.5f) self.Stun(Random.Range(2, (int)Mathf.Lerp(8f, 16f, data.sporePoison)));
                if (data.sporePoison >= 1f) self.Die();
            }
            else data.sporePoison -= sporepuffPoisonRecovery;
        }

        private static void Player_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
        {
            if (!IsConduit(self))
            {
                orig(self, grasp, eu);
                return;
            }
            if (self.grasps[grasp] == null || self.grasps[grasp].grabbed is JokeRifle) return;
            if (!PlayerCWT.TryGetData(self, out var data)) return;
            if (ModManager.MMF && self.room != null && MMF.cfgOldTongue.Value && self.grasps[grasp].grabbed is TubeWorm worm)
            {
                worm.Use();
                return;
            }
            if (self.grasps[grasp].grabbed is Weapon weapon)
            {
                IntVector2 throwDir = new(self.ThrowDirection, 0);
                bool isVertical = self.input[0].y != 0;
                bool isHorizontal = self.input[0].x != 0;

                if (isVertical && !isHorizontal)
                {
                    throwDir = new IntVector2(0, self.input[0].y);
                }
                Vector2 vector = self.firstChunk.pos + throwDir.ToVector2() * 15f + new Vector2(0f, 4f);
                if (self.room.GetTile(vector).Solid)
                {
                    vector = self.mainBodyChunk.pos;
                }
                if (self.grasps[grasp].grabbed is Spear spear)
                {
                    spear.Thrown(self, vector, self.mainBodyChunk.pos - throwDir.ToVector2() * 15f, throwDir, Mathf.Lerp(1f, 1.5f, self.Adrenaline), eu);
                    self.ThrownSpear(spear);
                }
                else
                {
                    weapon.Thrown(self, vector, self.mainBodyChunk.pos - throwDir.ToVector2() * 15f, throwDir, Mathf.Lerp(1f, 1.5f, self.Adrenaline), eu);
                }

                Vector2 throwBoost = -throwDir.ToVector2().normalized * throwboostVelocity;
                if (throwDir.x == 0 && throwDir.y == 0) throwBoost = new Vector2(-self.flipDirection, 0f) * throwboostVelocity;

                bool onGround = self.bodyChunks[0].ContactPoint.y == -1 || self.bodyChunks[1].ContactPoint.y == -1;
                if (onGround && !isVertical)
                {
                    Log.LogMessage("Dodge throw!");
                    throwBoost *= 0.8f;
                    self.animation = Player.AnimationIndex.Roll;
                    self.rollDirection = -throwDir.x;
                    self.rollCounter = 0;
                    self.SetRolling(true, 80);
                    data.dodgeRollWindow = maxDodgeRollWindow;
                    data.dodgeRollDirection = System.Math.Sign(-throwDir.x);
                }

                self.bodyChunks[0].vel += throwBoost;
                self.bodyChunks[1].vel += throwBoost * 0.8f;

                if (self.grasps[grasp].grabbed is ScavengerBomb bomb && throwDir.y == 1 && self.bodyMode != Player.BodyModeIndex.ZeroG)
                {
                    bomb.doNotTumbleAtLowSpeed = true;
                    bomb.throwModeFrames = 90;
                    bomb.firstChunk.vel *= 0.75f;
                }
                if (self.animation == Player.AnimationIndex.ClimbOnBeam && ModManager.MMF && MMF.cfgClimbingGrip.Value)
                {
                    self.bodyChunks[0].vel -= throwDir.ToVector2() * 2f;
                    self.bodyChunks[1].vel += throwDir.ToVector2() * 16f;
                }
                if (self.graphicsModule != null && self.graphicsModule is PlayerGraphics playerGraphics)
                {
                    playerGraphics.ThrowObject(grasp, self.grasps[grasp].grabbed);
                }
            }
            else if (self.grasps[grasp].grabbed is Frog)
            {
                if (!(self.grasps[grasp].grabbed as Frog).bloodBank)
                {
                    if (self.graphicsModule != null)
                    {
                        (self.graphicsModule as PlayerGraphics).ThrowObject(grasp, self.grasps[grasp].grabbed);
                    }
                    IntVector2 throwDir = new(self.ThrowDirection, 0);
                    bool isVertical = self.input[0].y != 0;
                    bool isHorizontal = self.input[0].x != 0;

                    if (isVertical && !isHorizontal)
                    {
                        throwDir = new IntVector2(0, self.input[0].y);
                    }
                    Vector2 vector = self.firstChunk.pos + throwDir.ToVector2() * 10f + new Vector2(0f, 4f);
                    if (self.room.GetTile(vector).Solid)
                    {
                        vector = self.mainBodyChunk.pos;
                    }
                    (self.grasps[grasp].grabbed as Frog).ImmuneToLatch = self;
                    (self.grasps[grasp].grabbed as Frog).throwLatch = true;
                    (self.grasps[grasp].grabbed as Frog).jumpStun = 40;
                    (self.grasps[grasp].grabbed as Frog).Thrown(self, vector + new Vector2(0f, 0.5f), self.mainBodyChunk.pos - throwDir.ToVector2() * 10f, throwDir, Mathf.Lerp(1.25f, 1.75f, self.Adrenaline), eu);
                }
                else
                {
                    (self.grasps[grasp].grabbed as Frog).ImmuneToLatch = self;
                    (self.grasps[grasp].grabbed as Frog).throwLatch = true;
                    (self.grasps[grasp].grabbed as Frog).jumpStun = 40;
                    self.TossObject(grasp, eu);
                }
            }
            else
            {
                self.TossObject(grasp, eu);
            }
            self.dontGrabStuff = 20;
            if (self.graphicsModule != null && self.graphicsModule is PlayerGraphics graphics)
            {
                graphics.LookAtObject(self.grasps[grasp].grabbed);
            }
            if (self.grasps[grasp].grabbed is PlayerCarryableItem playerCarryableItem)
            {
                playerCarryableItem.Forbid();
            }
            self.ReleaseGrasp(grasp);
            // TODO: Movement tech where throwing weapons during rollpounce gives you a gourmand midair roll
        }

        private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (!IsConduit(self)) return;
            if (!SpearCWT.TryGetData(spear, out var spearData)) return;
            spearData.thrownByProtag = true;
            // TODO: Make spears thrown by protag ignore water slowdown (see monitor lizard in lsf)
        }

        private static void Player_TerrainImpact(On.Player.orig_TerrainImpact orig, Player self, int chunk, RWCustom.IntVector2 direction, float speed, bool firstContact)
        {
            if (!IsConduit(self))
            {
                orig(self, chunk, direction, speed, firstContact);
                return;
            }

            bool rollBouncePounce = IsRolling(self) && self.wantToJump > 0 && direction.y < 0;
            self.SetRolling(false);

            orig(self, chunk, direction, speed, firstContact);

            if (rollBouncePounce)
            {
                Log.LogMessage("Roll bounce pounce!");
                self.room.PlaySound(SoundID.Slugcat_Sectret_Super_Wall_Jump, self.mainBodyChunk, loop: false, 1f, 1f);

                self.bodyChunks[1].pos = self.bodyChunks[0].pos;
                self.bodyChunks[0].pos += new Vector2(0f, 10f);

                self.bodyChunks[0].vel = new Vector2(self.bodyChunks[0].vel.x, 17f);
                self.bodyChunks[1].vel = new Vector2(self.bodyChunks[1].vel.x, 17f);

                self.animation = Player.AnimationIndex.RocketJump;
                self.room.ScreenMovement(self.mainBodyChunk.pos, new Vector2(0f, 1f), 0.1f);

                for (int i = 0; i < 7; i++)
                {
                    self.room.AddObject(new WaterDrip(self.mainBodyChunk.pos + new Vector2(0f, self.mainBodyChunk.rad), Custom.DegToVec(Random.value * 180f) * Mathf.Lerp(10f, 17f, Random.value), waterColor: false));
                }
                self.SetRolling(true);
            }
        }

        public static void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            if (IsConduit(self)) return;
            orig(self, grasp);
        }

        public static void Player_Stun(On.Player.orig_Stun orig, Player self, int st)
        {
            orig(self, st);
            if (!IsConduit(self)) return;
            if (!PlayerCWT.TryGetData(self, out var data)) return;
            data.rolling = false;
            data.rollCooldown = 40;
            data.rollAnimation = 0;
            data.breathTimer = 0;
        }

        public static bool Player_SpearStick(On.Player.orig_SpearStick orig, Player self, Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos appPos, Vector2 direction)
        {
            bool value = orig(self, source, dmg, chunk, appPos, direction);
            if (IsConduit(self))
            {
                if (IsRolling(self)) return false;
            }    
            return value;
        }

        public static void Player_Die(On.Player.orig_Die orig, Player self)
        {
            orig(self);
            if (!IsConduit(self)) return;
            if (!PlayerCWT.TryGetData(self, out var data)) return;
            data.rolling = false;
            data.rollCooldown = 0;
            data.rollAnimation = 0;
            data.breathTimer = 0;
            data.breathOrbs = 0;
        }


        public static void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            if (!IsConduit(self)) { orig(self, eu); return; }
            if (!PlayerCWT.TryGetData(self, out var data)) { orig(self, eu); return; }

            if (data.temporaryRollDuration > 0)
            {
                data.temporaryRollDuration--;
                if (data.temporaryRollDuration == 0)
                {
                    self.SetRolling(false);
                }
            }

            if (data.rolling)
            {
                self.standing = false;
                self.animation = Player.AnimationIndex.Roll;
                if (self.rollDirection == 0) self.rollDirection = self.flipDirection != 0 ? self.flipDirection : 1;
                self.bodyMode = Player.BodyModeIndex.Default;
            }

            orig(self, eu);

            // TODO: Make pressing jump + down cause you to start rolling
            // TODO: Disable slides and their variants
        }

        public static void ActivateBreathBubble(this Player self)
        {
            self.airInLungs = 0.5f;
            if (!PlayerCWT.TryGetData(self, out var data)) return;
            if (data.breathOrbs > 0)
            {
                data.breathOrbs--;
                data.breathTimer += maxBreathTime;
            }
        }

        public static void Lung_Update(On.Player.orig_LungUpdate orig, Player self)
        {
            orig(self);
            if (!IsConduit(self)) return;
            if (!PlayerCWT.TryGetData(self, out var data)) return;

            if (self.airInLungs - self.slugcatStats.drownThreshold < 0.1f)
            {
                self.ActivateBreathBubble();
            }
            if (data.breathTimer > 0)
            {
                data.breathTimer--;
                float currentBreath = (float)data.breathTimer / (float)maxBreathTime;
                if (Random.value < Mathf.InverseLerp(0f, 0.3f, currentBreath))
                {
                    Bubble bubble = new(self.firstChunk.pos + Custom.RNV() * Random.value * 4f, Custom.RNV() * Mathf.Lerp(6f, 16f, Random.value) * Mathf.InverseLerp(0f, 0.45f, currentBreath), bottomBubble: false, fakeWaterBubble: false);
                    self.room.AddObject(bubble);
                    bubble.age = 600 - Random.Range(20, Random.Range(30, 80));
                    for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                    {
                        if ((self.room.abstractRoom.creatures[i].rippleLayer != self.abstractPhysicalObject.rippleLayer && !self.room.abstractRoom.creatures[i].rippleBothSides && !self.abstractPhysicalObject.rippleBothSides) || self.room.abstractRoom.creatures[i].realizedCreature == null)
                        {
                            continue;
                        }
                        if (self.room.abstractRoom.creatures[i].realizedCreature is AirBreatherCreature && Custom.DistLess(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, 40f))
                        {
                            (self.room.abstractRoom.creatures[i].realizedCreature as AirBreatherCreature).lungs = Mathf.Min(1f, (self.room.abstractRoom.creatures[i].realizedCreature as AirBreatherCreature).lungs + 1f / 21f);
                        }
                        else if (self.room.abstractRoom.creatures[i].realizedCreature is Leech && !self.room.abstractRoom.creatures[i].realizedCreature.dead && Custom.DistLess(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, 70f))
                        {
                            float num = Mathf.InverseLerp(70f, 40f, Vector2.Distance(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos)) * self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.submersion;
                            if (Random.value < 0.007f * num)
                            {
                                self.room.abstractRoom.creatures[i].realizedCreature.Stun(16);
                            }
                            if (self.room.abstractRoom.creatures[i].realizedCreature.Consious && self.room.abstractRoom.creatures[i].realizedCreature.grasps[0] == null)
                            {
                                self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.vel += Custom.DirVec(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos) * num * Random.value * 12f;
                            }
                        }
                    }
                }
            }
        }

        public static void Player_JumpOnChunk(On.Player.orig_JumpOnChunk orig, Player self)
        {
            if (!IsConduit(self))
            {
                orig(self);
                return;
            }
            bool wasRolling = IsRolling(self);
            if (!wasRolling)
            {
                orig(self);
                return;
            }

            BodyChunk targetChunk = self.jumpChunk;
            Creature targetCreature = targetChunk?.owner as Creature;

            Vector2 playerPos = self.bodyChunks[0].pos;
            Vector2 creaturePos = targetChunk != null ? targetChunk.pos : playerPos;
            orig(self);

            if (targetCreature != null && targetCreature != self)
            {
                Log.LogMessage("Creature roll bounce!");
                Vector2 awayFromCreature = playerPos - creaturePos;
                if (awayFromCreature.magnitude < 0.01f) awayFromCreature = new Vector2(self.flipDirection != 0 ? self.flipDirection : 1f, 0f);
                awayFromCreature = awayFromCreature.normalized;

                Vector2 bounce = new(awayFromCreature.x * 7f, Mathf.Abs(awayFromCreature.y) * 5f + 6f);
                self.bodyChunks[0].vel = bounce;
                self.bodyChunks[1].vel = bounce * 0.7f;

                targetCreature.Violence(
                    self.bodyChunks[0],
                    awayFromCreature * -5f,
                    targetChunk,
                    null,
                    Creature.DamageType.Blunt,
                    0.6f,
                    1.2f
                );

                self.SetRolling(true);
            }
        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            if (!IsConduit(self))
            {
                orig(self);
                return;
            }
            if (!PlayerCWT.TryGetData(self, out var data)) return;

            bool wasCrouching = (self?.bodyMode == Player.BodyModeIndex.Crawl || self.animation == Player.AnimationIndex.DownOnFours);
            bool wasRolling = IsRolling(self);
            self.SetRolling(false);

            orig(self);

            if (wasCrouching && self.crawlTurnDelay < 0 && self.timeSinceInCorridorMode <= 0)
            {
                Log.LogMessage("Crouch spring!");
                float dir = self.flipDirection != 0 ? self.flipDirection : 1f;
                Vector2 lungeForce = new(dir * 10f, 6f);
                self.bodyChunks[0].vel += lungeForce;
                self.bodyChunks[1].vel += lungeForce * 0.6f;
                self.animation = Player.AnimationIndex.None;
                self.bodyMode = Player.BodyModeIndex.Default;
                self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, 0f - self.bodyChunks[1].rad), 3, 7f, 5f, 5.5f, 40f, new Color(1f, 1f, 1f, 0.5f)));
            }
            else if (data.dodgeRollWindow > 0)
            {
                Log.LogMessage("Dodge roll!");
                float dir = 1f;
                if (data.dodgeRollDirection < 0) dir = -1f;
                Vector2 lungeForce = new(dir * 6f, 5f);
                self.bodyChunks[0].vel += lungeForce;
                self.bodyChunks[1].vel += lungeForce * 0.8f;
                self.animation = Player.AnimationIndex.None;
                self.bodyMode = Player.BodyModeIndex.Default;
                self.standing = true;
                
            }
            else if (wasRolling)
            {
                Log.LogMessage("Roll bounce!");
                float dir = self.input[0].x != 0 ? self.input[0].x : self.flipDirection;
                if (dir == 0) dir = 1f;

                Vector2 bounce = new(dir * 7f, 10f);
                self.bodyChunks[0].vel = bounce;
                self.bodyChunks[1].vel = bounce * 0.8f;
                self.SetRolling(true, 80);
            }
            

        }

        public static bool Player_CanIPickThisUp(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
        {
            bool value = orig(self, obj);
            if (IsConduit(self) && obj is Player)
            {
                return false;
            }
            return value;
        }

        public static bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
        {
            bool value = orig(self, crit);
            // todo
            return value;
        }

        public static bool Player_CanBeSwallowed(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            bool value = orig(self, testObj);
            if (IsConduit(self))
            {
                return false;
            }
            return value;
        }

        public static bool Player_CanBeGrabbed(On.Player.orig_CanBeGrabbed orig, Player self, Creature grabber)
        {
            bool value = orig(self, grabber);
            if (IsRolling(self))
            {
                return false;
            }
            return value;
        }

        public static void Player_Blink(On.Player.orig_Blink orig, Player self, int blink)
        {
            orig(self, blink);
            if (IsConduit(self))
            {
                if (self.graphicsModule != null)
                {
                    (self.graphicsModule as PlayerGraphics).blink = 0;
                }
            }
        }

        public static bool Player_AllowGrabbingBatflys(On.Player.orig_AllowGrabbingBatflys orig, Player self)
        {
            bool result = orig(self);
            if (IsConduit(self))
            {
                return false;
            }
            return result;
        }
    }
}
