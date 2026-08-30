using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace loremiscExpansion.NPCs.Boris
{
    public class Boris : UpdatableAndDeletable, IDrawable
    {
        public PlacedObject placedObject;

        public int animationCounter;

        public const int firstBodySprite = 2;

        public const int distortionSprite = 1;

        public const int lightSprite = 0;

        public const int totalSprites = 3;
        
        public Vector2 pos;
        
        public PositionedSoundEmitter voice = null;

        public bool dead = false;

        public Boris(PlacedObject placedObject, Room room) : base()
        {
            this.room = room;
        }

        public void MeetingFinished()
        {
            if (this?.room?.game?.GetStorySession?.saveState != null)
            {
                BorisState state = room.game.GetStorySession.saveState.GetBorisState();
                state.cyclesSinceLastEncounter = 0;
                state.encounters++;
                room.game.GetStorySession.saveState.SetBorisState(state);
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (slatedForDeletetion) return;
            // Graphics and fadeout are their own classes
        }


        void IDrawable.AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            newContatiner ??= rCam.ReturnFContainer("Items");
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].RemoveFromContainer();
                if (i == distortionSprite)
                {
                    rCam.ReturnFContainer("Bloom").AddChild(sLeaser.sprites[i]);
                }
                else if (i == lightSprite)
                {
                    rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                }
                else
                {
                    newContatiner.AddChild(sLeaser.sprites[i]);
                }
            }
        }

        void IDrawable.ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            
        }

        void IDrawable.DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            
        }

        void IDrawable.InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            
        }
    }
}
