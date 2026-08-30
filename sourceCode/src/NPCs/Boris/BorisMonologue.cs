using System.Linq;
using HUD;
using IL.Watcher;
using UnityEngine;

namespace loremiscExpansion.NPCs.Boris;
public class BorisMonologue : Conversation.IOwnAConversation
{
    public class Stage : ExtEnum<Stage>
    {
        public static Stage Idle = new(nameof(Idle), register: true);

        public static Stage Noticed = new(nameof(Noticed), register: true);

        public static Stage Talking = new(nameof(Talking), register: true);

        public static Stage Pause = new(nameof(Pause), register: true);

        public static Stage Flee = new(nameof(Flee), register: true);

        public static Stage Disappear = new(nameof(Disappear), register: true);

        public Stage(string value, bool register = false) : base(value, register)
        {

        }
    }

    public Boris owner;

    public const int noticePlayerDelay = 60;

    public const int speakDelay = 100;

    public const int disappearDelay = 160;

    public Stage stage = Stage.Idle;

    public int timeInStage;

    public BorisConversation conversation;

    private int onScreenCounter;

    public bool NoticedPlayer => stage != Stage.Idle;

    public Vector2? pos = null;

    public bool IsOnScreen
    {
        get
        {
            int num = owner.room.CameraViewingPoint(owner.pos);
            foreach (var camera in owner.room.game.cameras) if (camera.room == owner.room && camera.currentCameraPosition == num) return true;
            return false;
        }
    }

    public bool AnyCameraInRoom => owner.room.game.cameras.Any(e => e.room == owner.room);

    public DialogBox DialogBox
    {
        get
        {
            if (conversation != null)
            {
                return conversation.dialogBox;
            }
            HUD.HUD hud = owner.room.game.cameras[0].hud;
            if (hud.dialogBox == null)
            {
                hud.InitDialogBox();
            }
            return hud.dialogBox;
        }
    }

    public BorisMonologue(Boris owner)
    {
        this.owner = owner;
    }

    public void Update()
    {
        timeInStage++;
        conversation?.Update();
        if (stage == Stage.Idle)
        {
            if (IsOnScreen)
            {
                onScreenCounter++;
            }
            else
            {
                onScreenCounter = 0;
            }
            if (onScreenCounter > 60)
            {
                NextStage();
            }
        }
        else if (stage == Stage.Noticed)
        {
            if (timeInStage > 100)
            {
                StartMonologue();
                NextStage();
            }
        }
        else if (stage == Stage.Talking)
        {
            if (!AnyCameraInRoom)
            {
                //owner.StopSpeaking();
                conversation?.Destroy();
                conversation = null;
                stage = Stage.Idle;
                onScreenCounter = 0;
                timeInStage = 0;
            }
            else
            {
                if (!conversation.slatedForDeletion)
                {
                    return;
                }
                conversation = null;
                Player player = null;
                float num = 0f;
                for (int i = 0; i < owner.room.game.Players.Count; i++)
                {
                    if (owner.room.game.Players[i].realizedCreature != null)
                    {
                        Player player2 = owner.room.game.Players[i].realizedCreature as Player;
                        if (player2.controller == null)
                        {
                            player2.controller = new Player.NullController();
                        }
                        if (player2.room == owner.room && (player == null || Vector2.Distance(player2.mainBodyChunk.pos, owner.pos) < num))
                        {
                            player = player2;
                            num = Vector2.Distance(player2.mainBodyChunk.pos, owner.pos);
                        }
                    }
                }
                if (player != null)
                {
                    owner.room.game.cameras[0].EnterCutsceneMode(player.abstractCreature, RoomCamera.CameraCutsceneType.Standard);
                }
                NextStage();
            }
        }
        else if (stage == Stage.Pause)
        {
            if (timeInStage > 160)
            {
                NextStage();
            }
        }
        else
        {
            _ = stage == Stage.Disappear;
        }
    }

    public Conversation.ID GetConversationID()
    {
        return null;
    }

    public void StartMonologue()
    {
        conversation = new BorisConversation(this, GetConversationID(), DialogBox);
        //owner.Speak(conversation.Voiceline);
    }

    public void StartVanish()
    {
        //owner.StartDeactivate();
    }

    public virtual string ReplaceParts(string s)
    {
        return s;
    }

    public virtual void SpecialEvent(string eventName)
    {
    }

    public void NextStage()
    {
        if (stage == Stage.Idle)
        {
            stage = Stage.Noticed;
        }
        else if (stage == Stage.Noticed)
        {
            stage = Stage.Talking;
        }
        else if (stage == Stage.Talking)
        {
            stage = Stage.Pause;
        }
        else
        {
            if (!(stage == Stage.Pause))
            {
                return;
            }
            stage = Stage.Disappear;
        }
        timeInStage = 0;
    }
}
