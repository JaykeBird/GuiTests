using SolidShineUi;
using SolidShineUi.KeyboardShortcuts;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows;

namespace DropTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        IDataObject? loaded = null;

        public MainWindow()
        {
            InitializeComponent();

            KeyboardShortcutHandler ksh = new KeyboardShortcutHandler(this);
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.Ctrl, System.Windows.Input.Key.V,
                new ActionKeyAction(PasteData, "PasteData", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.Ctrl, System.Windows.Input.Key.C,
                new ActionKeyAction(CopyDoo, "CopyDoo", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, System.Windows.Input.Key.Delete,
                new ActionKeyAction(Clear, "Clear", this));

            SsuiTheme = new SsuiAppTheme(SystemColors.AccentColorLight1);
        }

        #region Drag/Drop Event Handlers

        private void selPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void selPanel_DragOver(object sender, DragEventArgs e)
        {

        }

        private void selPanel_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void selPanel_Drop(object sender, DragEventArgs e)
        {
            IDataObject doo = e.Data;

            RenderDataObject(doo);
        }

        #endregion

        void CopyDoo()
        {
            if (loaded != null)
            {
                Clipboard.SetDataObject(loaded);
            }
        }

        void Clear()
        {
            selPanel.Items.Clear();
            txtExplain.Visibility = Visibility.Visible;
        }

        void PasteData()
        {
            var doo = Clipboard.GetDataObject();

            if (doo != null)
            {
                RenderDataObject(doo);
            }
        }

        void RenderDataObject(IDataObject doo)
        {
            selPanel.Items.Clear();
            txtExplain.Visibility = Visibility.Collapsed;

            loaded = doo;

            foreach (string format in doo.GetFormats())
            {
                object data = doo.GetData(format);

                FlatButton si = new FlatButton();
                si.Content = format;
                si.Tag = data;
                si.ToolTip = "Click to copy just this format";
                si.Click += button_Click;
                selPanel.Items.Add(si);

                SelectableItem sdataType = new SelectableItem(data?.GetType().FullName ?? "null", null, 12);
                selPanel.Items.Add(sdataType);

                // try to print out the data
                if (data is string[] sa)
                {
                    SelectableItem siData = new SelectableItem(string.Join(',', sa), null, 12);
                    selPanel.Items.Add(siData);
                }
                //else if (data is MemoryStream ms)
                //{
                //    byte[] bytes = ms.GetBuffer();

                //    SelectableItem siData = new SelectableItem(PrintBytes(bytes, 50), null, 12);
                //    selPanel.Items.Add(siData);
                //}
                else
                {
                    SelectableItem siData = new SelectableItem(data?.ToString() ?? "", null, 12);
                    selPanel.Items.Add(siData);
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FlatButton fb && fb.Content is string s && fb.Tag != null)
            {
                DataObject doo = new DataObject(s, fb.Tag);
                Clipboard.SetDataObject(doo, true);
            }
        }

        public static string PrintBytes(byte[] byteArray, int? maxLength = null)
        {
            byte[] ba = byteArray;
            if (maxLength != null && byteArray.Length > maxLength)
            {
                ba = byteArray.Take((int)maxLength).ToArray();
            }

            var sb = new StringBuilder("new byte[] { ");
            for (var i = 0; i < ba.Length; i++)
            {
                var b = ba[i];
                sb.Append(b);
                if (i < ba.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}