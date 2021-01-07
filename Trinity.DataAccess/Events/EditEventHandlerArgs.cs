using Trinity.DataAccess.LobModels;

namespace Trinity.DataAccess.Events
{
    public class EditEventHandlerArgs
    {
        public ModelEditType EditType { get; set; }

        public EditEventHandlerArgs(ModelEditType editType)
        {
            this.EditType = editType;
        }
    }
}