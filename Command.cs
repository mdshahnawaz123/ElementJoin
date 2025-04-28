using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ElementJoin
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Collect all Structural Columns
            var columns = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .ToElements();

            // Collect all Structural Foundations
            var foundations = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .WhereElementIsNotElementType()
                .ToElements();

            if (!columns.Any() || !foundations.Any())
            {
                TaskDialog.Show("Warning", "No columns or foundations found.");
                return Result.Failed;
            }

            using (Transaction trans = new Transaction(doc, "Join Columns to Foundations"))
            {
                trans.Start();

                foreach (var column in columns)
                {
                    foreach (var foundation in foundations)
                    {
                        // Check if they can be joined first
                        if (JoinGeometryUtils.AreElementsJoined(doc, column, foundation))
                        {
                            // Already joined, maybe skip or unjoin first
                            continue;
                        }

                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, column, foundation);
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("Error", $"Failed to join {column.Name} with {foundation.Name}: {ex.Message}");
                        }
                    }
                }

                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}

