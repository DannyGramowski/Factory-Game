
namespace Factory.Units.Actions {
    public interface IAction {
        public void OnInitiate();
        public void OnTick();
        public bool IsFinished();
        public string ActionName();
        public void OnExit();
    }
}