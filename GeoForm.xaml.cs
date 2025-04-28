using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElementJoin
{
    public partial class GeoForm : Window
    {
        private Document doc;
        private UIDocument uidoc;

        private JoinExternalEventHandler joinHandler;
        private ExternalEvent joinEvent;

        private UnjoinExternalEventHandler unjoinHandler;
        private ExternalEvent unjoinEvent;

        private SwitchJoinOrderExternalEventHandler switchHandler;
        private ExternalEvent switchEvent;

        public GeoForm(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;

            joinHandler = new JoinExternalEventHandler();
            joinEvent = ExternalEvent.Create(joinHandler);

            unjoinHandler = new UnjoinExternalEventHandler();
            unjoinEvent = ExternalEvent.Create(unjoinHandler);

            switchHandler = new SwitchJoinOrderExternalEventHandler();
            switchEvent = ExternalEvent.Create(switchHandler);

            var categories = doc.Settings.Categories
                .Cast<Category>()
                .Where(c => c.CategoryType == CategoryType.Model && !c.IsTagCategory)
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var category in categories)
            {
                CBCategory1.Items.Add(category);
                CBCategory2.Items.Add(category);
            }

            CBCategory1.DisplayMemberPath = "Name";
            CBCategory2.DisplayMemberPath = "Name";
        }

        private void Join_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItem1 = CBCategory1.SelectedItem as Category;
            var selectedItem2 = CBCategory2.SelectedItem as Category;

            if (selectedItem1 == null || selectedItem2 == null)
            {
                TaskDialog.Show("Warning", "Please select both categories.");
                return;
            }

            joinHandler.Doc = doc;
            joinHandler.Category1 = selectedItem1;
            joinHandler.Category2 = selectedItem2;

            joinEvent.Raise();
        }

        private void UnJoin_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItem1 = CBCategory1.SelectedItem as Category;
            var selectedItem2 = CBCategory2.SelectedItem as Category;

            if (selectedItem1 == null || selectedItem2 == null)
            {
                TaskDialog.Show("Warning", "Please select both categories.");
                return;
            }

            unjoinHandler.Doc = doc;
            unjoinHandler.Category1 = selectedItem1;
            unjoinHandler.Category2 = selectedItem2;

            unjoinEvent.Raise();
        }

        private void SwitchJoinOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItem1 = CBCategory1.SelectedItem as Category;
            var selectedItem2 = CBCategory2.SelectedItem as Category;

            if (selectedItem1 == null || selectedItem2 == null)
            {
                TaskDialog.Show("Warning", "Please select both categories.");
                return;
            }

            switchHandler.Doc = doc;
            switchHandler.Category1 = selectedItem1;
            switchHandler.Category2 = selectedItem2;

            switchEvent.Raise();
        }
    }
}
