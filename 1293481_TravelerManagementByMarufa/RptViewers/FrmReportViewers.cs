using _1293481_TravelerManagementByMarufa.Reports;
using _1293481_TravelerManagementByMarufa.Repositories;
using _1293481_TravelerManagementByMarufa.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1293481_TravelerManagementByMarufa.RptViewers
{
    public partial class FrmReportViewers : Form
    {
        IEnumerable<TravelerInfoViewModel> myList = new List<TravelerInfoViewModel>();
        TravelerRepo repo = new TravelerRepo();
        public FrmReportViewers()
        {
            InitializeComponent();
        }
        
      
        public FrmReportViewers(IEnumerable<TravelerInfoViewModel> list)
        {
            InitializeComponent();
            myList = list;
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. Initialize the report object (the .rpt file you designed)
                rptTravelerInfo rptObj = new rptTravelerInfo();

                // 2. Bind the processed list (myList) to the report
                // Crystal Reports will automatically match the properties in your 
                // TravelerInfoViewModel to the fields in the report design.
                rptObj.SetDataSource(myList);

                // 3. Set the viewer's source and refresh
                crystalReportViewer1.ReportSource = rptObj;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Report Load Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
}
