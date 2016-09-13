using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using ResxTranslator.ResourceOperations;

namespace ResxTranslator.Controls
{
    public partial class ResourceFilter : UserControl
    {
        private string _filter;
        private string _filterText;

        public ResourceFilter()
        {
            InitializeComponent();

            _filter = null;
            _filterText = "";
        }

        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                if (textBox1.Text != value)
                    textBox1.Text = value;

                UpdateFilter();
            }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                OnResourceFilterChanged();
            }
        }

        public IEnumerable<string> EnabledColumns
            => listView1.CheckedItems.Cast<ListViewItem>().Select(x => x.Text);

        public IEnumerable<string> DisabledColumns
        {
            get
            {
                var enabledItems = listView1.CheckedItems.Cast<ListViewItem>();
                return listView1.Items.Cast<ListViewItem>()
                    .Where(x => !enabledItems.Contains(x))
                    .Select(x => x.Text);
            }
        }

        public void RefreshColumnNames(IEnumerable<string> columnNames, bool perserveState)
        {
            List<string> storedDisabled = null;
            if (perserveState)
                storedDisabled = DisabledColumns.ToList();

            listView1.SuspendLayout();
            listView1.BeginUpdate();

            listView1.Items.Clear();

            foreach (var columnName in columnNames)
            {
                listView1.Items.Add(new ListViewItem(new[] { columnName })
                {
                    Checked = !perserveState || !storedDisabled.Contains(columnName)
                });
            }

            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);

            listView1.EndUpdate();
            listView1.ResumeLayout();

            UpdateFilter();
        }

        public ResourceHolder CurrentResource { get; set; }

        public event EventHandler ResourceFilterChanged;

        protected virtual void OnResourceFilterChanged()
        {
            ResourceFilterChanged?.Invoke(this, EventArgs.Empty);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterText = textBox1.Text;
        }

        public void UpdateFilter()
        {
            StringBuilder sb = new StringBuilder();

            var columns = EnabledColumns.ToList();

            bool first = true;
            foreach (var column in EnabledColumns)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(" OR ");
                }

                sb.AppendFormat("{0} LIKE '*{1}*'", column, EscapeExpression(FilterText));
            }

            Filter = sb.ToString();
        }

        private string EscapeExpression(string input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var c in input)
            {
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateFilter();
        }
    }
}
