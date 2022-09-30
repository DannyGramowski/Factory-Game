
namespace Factory.Units.Actions {
    public interface IAction {
        public void OnInitiate(object[] param);
        public void OnTick(object[] param);
        public bool IsFinished();
        public string ActionName();
        public void OnExit(object[] param);
    }
}