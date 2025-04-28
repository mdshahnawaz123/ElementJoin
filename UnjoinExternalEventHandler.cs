using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace ElementJoin
{
    public class UnjoinExternalEventHandler : IExternalEventHandler
    {
        public Document Doc;
        public Category Category1;
        public Category Category2;

        public void Execute(UIApplication app)
        {
            if (Doc == null || Category1 == null || Category2 == null)
                return;

            var elements1 = new FilteredElementCollector(Doc)
                .OfCategoryId(Category1.Id)
                .WhereElementIsNotElementType()
                .ToElements();

            var elements2 = new FilteredElementCollector(Doc)
                .OfCategoryId(Category2.Id)
                .WhereElementIsNotElementType()
                .ToElements();

            using (Transaction tx = new Transaction(Doc, "Unjoin Elements"))
            {
                tx.Start();

                foreach (var element1 in elements1)
                {
                    foreach (var element2 in elements2)
                    {
                        try
                        {
                            if (JoinGeometryUtils.AreElementsJoined(Doc, element1, element2))
                            {
                                JoinGeometryUtils.UnjoinGeometry(Doc, element1, element2);
                            }
                        }
                        catch
                        {
                            // Optional: Handle specific exceptions
                        }
                    }
                }

                tx.Commit();
            }
        }

        public string GetName()
        {
            return "Unjoin Elements External Event";
        }
    }
}

