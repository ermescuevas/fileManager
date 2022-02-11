using Ermes.FileManager.Core;
using Ermes.FileManager.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ermes.FileManager.UI
{
    public partial class BreadcrumbGeneratorForm : Form
    {
        private readonly BreadcrumbService _breadcrumbService;
        private readonly FileProvider _fileProvider;
        public BreadcrumbGeneratorForm()
        {
            InitializeComponent();
            _breadcrumbService = new BreadcrumbService();
            _fileProvider = new FileProvider();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

            var listBreadCrumbs = new List<Breadcrumb>();
            string breadCrumbStructure = "";
            for (int i = 1; i < 6; i++)
            {
                var name = Controls.Find($"txtNamePosition{i}", false).FirstOrDefault() as TextBox;
                var url = Controls.Find($"txtUrlPosition{i}", false).FirstOrDefault() as TextBox;
                if (name != null && url != null && !string.IsNullOrEmpty(name.Text) && !string.IsNullOrEmpty(url.Text))
                {
                    listBreadCrumbs.Add(new Breadcrumb { Name = name.Text, Url = url.Text });
                }
            }

            if (listBreadCrumbs.Count > 0)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (ckbGenerateHtml.Checked)
                    {
                        breadCrumbStructure = _breadcrumbService.CreateHtmlStructure(listBreadCrumbs, ckbGenerateCss.Checked);
                    }
                    else
                    {
                        if (ckbGenerateCss.Checked)
                            breadCrumbStructure += _breadcrumbService.GetCssString();
                        breadCrumbStructure += _breadcrumbService.GetBreadcrumb(listBreadCrumbs);
                    }

                    _fileProvider.CreateFileFromText(saveFileDialog1.FileName, breadCrumbStructure, Encoding.Default);
                    MessageBox.Show($"File generate correctly in the location {saveFileDialog1.FileName}");
                    ClearFields();
                }
            }
            else
            {
                MessageBox.Show("You must fill in at least one url and name field to continue");
            }
        }

        #region Utilities

        private void ClearFields()
        {
            for (int i = 1; i < 6; i++)
            {
                var name = Controls.Find($"txtNamePosition{i}", false).FirstOrDefault() as TextBox;
                var url = Controls.Find($"txtUrlPosition{i}", false).FirstOrDefault() as TextBox;
                name.Text = "";
                url.Text = "";
            }
        }

        #endregion
    }
}