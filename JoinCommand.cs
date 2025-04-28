using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ElementJoin
{
    [Transaction(TransactionMode.Manual)]
    public class JoinCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            GeoForm form = new GeoForm(doc, uidoc);
            form.Show();

            return Result.Succeeded;
        }
    }
}

