
namespace Monolith.Util
{
    public enum DrawStageType
    {
        None,
        Start,
        Scene,
        DrawManager,
        PostProcessing,
        UI,
        End
    }


    public class DrawStage
    {
        public DrawStageType StageType { get; }
        public bool IsActive { get; private set; }

        public DrawStage(DrawStageType stageType)
        {
            StageType = stageType;
        }

        public void Begin()
        {
            IsActive = true;
        }

        public void End()
        {
            IsActive = false;
        }
    }
}