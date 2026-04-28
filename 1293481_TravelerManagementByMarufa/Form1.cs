using _1293481_TravelerManagementByMarufa.Entities;
using _1293481_TravelerManagementByMarufa.Repositories;
using _1293481_TravelerManagementByMarufa.RptViewers;
using _1293481_TravelerManagementByMarufa.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace _1293481_TravelerManagementByMarufa
{
    public partial class Form1 : Form
    {
        private readonly OpenFileDialog ofd = new OpenFileDialog();
        private bool isDefaultImage = true;
        private string previousImage = "";
        private int intTravelerId = 0;

        private Traveler traveler = new Traveler();
        private readonly TravelerRepo repo = new TravelerRepo();
        public List<TravelPlan> TravelPlans { get; set; } = new List<TravelPlan>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadTripPackage();
            LoadTravelerGrid();
            ClearAll();

        }
        private void ClearAll()
        {
            string noImagePath = Path.Combine(Application.StartupPath, "images", "noimage.png");

            if (File.Exists(noImagePath))
            {
                pbUpload.SizeMode = PictureBoxSizeMode.Zoom;

                byte[] imageBytes = File.ReadAllBytes(noImagePath);
                using (var ms = new MemoryStream(imageBytes))
                {
                    pbUpload.Image = Image.FromStream(ms);
                }
            }

            isDefaultImage = true;
            previousImage = "";
            chkIsRegular.Checked = false;
            txtName.Clear();
            mtbMobile.Clear();
            mtbNID.Clear();
            txtDeparture.Clear();

            dgvNewTravelPlan.DataSource = null;
            dgvTravelPlan.DataSource = null;
            btnSave.Text = "Save";
            intTravelerId = 0;
            traveler = new Traveler();
        }

        private void LoadTripPackage()
        {
            DataTable dt = repo.GetAllTripPackages();
            DataRow topRow = dt.NewRow();
            topRow[0] = 0;
            topRow[1] = "--Select Trip package--";
            dt.Rows.InsertAt(topRow, 0);

            cmbTripPackage.DataSource = dt;
            cmbTripPackage.DisplayMember = "TripPackageName";
            cmbTripPackage.ValueMember = "TripPackageId";

        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Images(.jpg,.png)|*.jpg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (var stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {

                    using (var tempImage = Image.FromStream(stream))
                    {
                        pbUpload.Image = new Bitmap(tempImage);
                    }
                }
                pbUpload.SizeMode = PictureBoxSizeMode.Zoom;
                isDefaultImage = false;
                traveler.ImageUrl = ofd.FileName;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            using (var stream = new FileStream(Application.StartupPath + "\\images\\noimage.png", FileMode.Open, FileAccess.Read))
            {
                pbUpload.Image = Image.FromStream(stream);
            }
            isDefaultImage = true;
            previousImage = "";
        }
        public bool IsValidated()
        {
            return !string.IsNullOrWhiteSpace(txtName.Text) &&
                   cmbTripPackage.SelectedIndex > 0 &&
                   !string.IsNullOrWhiteSpace(mtbMobile.Text);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidated())
            {
                MessageBox.Show("Please provide all required information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                traveler.TravelerId = intTravelerId;
                traveler.TravelerName = txtName.Text.Trim();
                traveler.MobileNo = mtbMobile.Text;
                traveler.NID = mtbNID.Text;
                traveler.IsRegular = chkIsRegular.Checked;
                traveler.DepartureFrom = txtDeparture.Text.Trim();
                traveler.NoOfPersonsToGo = (int)nudPassangerNo.Value;
                traveler.TravelStartDate = dtpStartDate.Value;
                traveler.TravelEndDate = dtpEndDate.Value;

                if (traveler.TravelPlans == null) traveler.TravelPlans = new List<TravelPlan>();
                else traveler.TravelPlans.Clear();

                foreach (DataGridViewRow row in dgvNewTravelPlan.Rows)
                {
                    if (row.IsNewRow) continue;

                    TravelPlan plan = new TravelPlan
                    {
                        TravelPlanId = row.Cells["TravelPlanId"].Value == null ? 0 : Convert.ToInt32(row.Cells["TravelPlanId"].Value),

                        TripPackageId = Convert.ToInt32(row.Cells["TripPackageId"].Value),
                        DesiredPlacesToVisit = row.Cells["DesiredPlacesToVisit"].Value?.ToString(),
                        TravelMode = row.Cells["TravelMode"].Value?.ToString(),
                        EstimatedHour = Convert.ToInt32(row.Cells["EstimatedHour"].Value)
                    };

                    traveler.TravelPlans.Add(plan);
                }

                if (isDefaultImage)
                    traveler.ImageUrl = "noimage.png";
                else if (!string.IsNullOrEmpty(ofd.FileName))
                {
                    if (intTravelerId > 0 && !string.IsNullOrEmpty(previousImage) && previousImage != "noimage.png")
                        DeleteImageFile(previousImage);

                    traveler.ImageUrl = SaveImage(ofd.FileName);
                }
                else
                    traveler.ImageUrl = previousImage;

                int result = (intTravelerId == 0) ? repo.SaveTraveler(traveler) : repo.UpdateTraveler(traveler);

                if (result > 0)
                {
                    MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTravelerGrid();
                    ClearAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private string SaveImage(string imgPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(imgPath);
            string ext = Path.GetExtension(imgPath);
            fileName = fileName.Length <= 15 ? fileName : fileName.Substring(0, 15);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + ext;
            string directoryPath = Path.Combine(Application.StartupPath, "images");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fullSavePath = Path.Combine(directoryPath, fileName);

            File.Copy(imgPath, fullSavePath, true);

            return fileName;
        }


        private void DeleteImageFile(string previousImage)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "images", previousImage);
                if (File.Exists(path) && previousImage != "noimage.png")
                {
                    pbUpload.Image?.Dispose();
                    pbUpload.Image = null;
                    File.Delete(path);
                }
            }
            catch (Exception ex) { MessageBox.Show("Image Delete Error: " + ex.Message); }
        }

        private void LoadTravelerGrid()
        {
            DataTable dt = repo.GetAllTravelers();

            if (!dt.Columns.Contains("Image"))
                dt.Columns.Add("Image", typeof(byte[]));

            foreach (DataRow dr in dt.Rows)
            {
                string imgName = dr["ImageUrl"].ToString();
                string imagePath = Path.Combine(Application.StartupPath, "images", imgName);
                string defaultPath = Path.Combine(Application.StartupPath, "images", "noimage.png");

                try
                {
                    dr["Image"] = File.Exists(imagePath) ? File.ReadAllBytes(imagePath) : File.ReadAllBytes(defaultPath);
                }
                catch { dr["Image"] = File.ReadAllBytes(defaultPath); }
            }

            dgvTraveler.DataSource = null;
            dgvTraveler.Columns.Clear();
            dgvTraveler.DataSource = dt;

            AddGridButton("Details", "Details");
            AddGridButton("Edit", "Edit");
            AddGridButton("Delete", "Delete");

            dgvTraveler.Columns["Details"].DisplayIndex = 0;
            dgvTraveler.Columns["Edit"].DisplayIndex = 1;
            dgvTraveler.Columns["Delete"].DisplayIndex = 2;

            dgvTraveler.RowTemplate.Height = 80;
            if (dgvTraveler.Columns["Image"] is DataGridViewImageColumn imgCol)
            {
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgCol.DisplayIndex = 3;
                imgCol.HeaderText = "Traveler Photo";
            }

            if (dgvTraveler.Columns.Contains("TravelerId")) dgvTraveler.Columns["TravelerId"].Visible = false;
            if (dgvTraveler.Columns.Contains("ImageUrl")) dgvTraveler.Columns["ImageUrl"].Visible = false;
        }

        private void AddGridButton(string name, string text)
        {
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                HeaderText = text,
                UseColumnTextForButtonValue = true
            };
            dgvTraveler.Columns.Add(btn);
        }
        private void btnNewTravelPlanSave_Click(object sender, EventArgs e)
        {
            try
            {
                TravelPlan travelPlan = new TravelPlan
                {
                    TripPackageId = Convert.ToInt32(cmbTripPackage.SelectedValue),

                    DesiredPlacesToVisit = txtTouristSpots.Text.Trim(),
                    TravelMode = txtTravelMode.Text.Trim(), 
                    EstimatedHour = Convert.ToInt32(nudEstimatedHours.Value)
                };


                traveler.TravelPlans.Add(travelPlan);
                LoadTravelPlanGrid(traveler.TravelPlans);

                txtTouristSpots.Clear();
                txtTravelMode.Text = "Any Ride"; 
                nudEstimatedHours.Value = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding plan: " + ex.Message);
            }
        }

        private void LoadTravelPlanGrid(List<TravelPlan> travelPlans)
        {
            dgvNewTravelPlan.DataSource = null;
            dgvNewTravelPlan.Columns.Clear();
            dgvNewTravelPlan.AutoGenerateColumns = false;

            dgvNewTravelPlan.Columns.Add("TravelPlanId", "Plan ID");
            dgvNewTravelPlan.Columns["TravelPlanId"].Visible = false;

            dgvNewTravelPlan.Columns.Add("TripPackageId", "Trip Package");
            dgvNewTravelPlan.Columns["TripPackageId"].Visible = false;

            dgvNewTravelPlan.Columns.Add("DesiredPlacesToVisit", "Tourist Spots");
            dgvNewTravelPlan.Columns.Add("TravelMode", "Travel Mode");
            dgvNewTravelPlan.Columns.Add("EstimatedHour", "Estimated Hours");

            foreach (var plan in travelPlans)
            {
                int rowIndex = dgvNewTravelPlan.Rows.Add();
                DataGridViewRow row = dgvNewTravelPlan.Rows[rowIndex];

                row.Cells["TravelPlanId"].Value = plan.TravelPlanId;
                row.Cells["TripPackageId"].Value = plan.TripPackageId;
                row.Cells["DesiredPlacesToVisit"].Value = plan.DesiredPlacesToVisit;
                row.Cells["TravelMode"].Value = plan.TravelMode;
                row.Cells["EstimatedHour"].Value = plan.EstimatedHour;

                row.Tag = plan;
            }

            AddGridButtonToControl(dgvNewTravelPlan, "Delete", "Delete");
            ClearTravelPlanInputs();
        }
        private void ClearTravelPlanInputs()
        {
            
            txtTouristSpots.Clear();
            txtTravelMode.Text = "Any Ride";

            nudEstimatedHours.Value = 1;

            cmbTripPackage.Focus();
        }
        

        private void AddGridButtonToControl(DataGridView dgvNewTravelPlan, string name, string text)
        {
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                HeaderText = text,
                Width = 60,
                UseColumnTextForButtonValue = true
            };
            dgvNewTravelPlan.Columns.Add(btn);
        }

        private void dgvNewTravelPlan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvNewTravelPlan.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this travel plan?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        int tId = Convert.ToInt32(dgvNewTravelPlan.Rows[e.RowIndex].Cells["TravelerId"].Value);
                        int tpId = Convert.ToInt32(dgvNewTravelPlan.Rows[e.RowIndex].Cells["TravelPlanId"].Value);

                        if (tId > 0 && tpId > 0)
                        {
                            repo.DeleteTavelPlanByTravelerId(tId, tpId);
                        }
                    }
                    catch { }

                    if (traveler.TravelPlans.Count > e.RowIndex) traveler.TravelPlans.RemoveAt(e.RowIndex);
                    LoadTravelPlanGrid(traveler.TravelPlans);
                }
            }
        }

        private void dgvTraveler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int tId = Convert.ToInt32(dgvTraveler.Rows[e.RowIndex].Cells["TravelerId"].Value);

                string command = dgvTraveler.Columns[e.ColumnIndex].Name;

                switch (command)
                {
                    case "Details":
                        LoadTravelPlanGridForDetails(tId);
                        break;

                    case "Edit":
                        EditTravelerInfo(tId);
                        break;

                    case "Delete":
                        if (MessageBox.Show("Are you sure you want to delete this traveler and all their travel plans?",
                                            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            DeleteTravelerInfo(tId);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void LoadTravelPlanGridForDetails(int travelerId)
        {
            dgvTravelPlan.DataSource = null;
            dgvTravelPlan.Columns.Clear();

            DataTable dt = repo.GetPlansByTravelerId(travelerId);

            dgvTravelPlan.DataSource = dt;

            if (dgvTravelPlan.Columns.Contains("TravelerId"))
                dgvTravelPlan.Columns["TravelerId"].Visible = false;

            if (dgvTravelPlan.Columns.Contains("TripPackageId"))
                dgvTravelPlan.Columns["TripPackageId"].Visible = false;
        }


        private void DeleteTravelerInfo(int tId)
        {
            DataTable dt = repo.GetTravelerById(tId);
            if (dt.Rows.Count > 0) DeleteImageFile(dt.Rows[0]["ImageUrl"].ToString());

            repo.DeleteTravelerInfoByTravelerId(tId);
            repo.DeleteTraveler(tId);

            LoadTravelerGrid();
            ClearAll();
            
        }

        private void EditTravelerInfo(int travelerId)
        {
            
            DataTable dt = repo.GetTravelerById(travelerId);
            if (dt == null || dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];

            
            btnSave.Text = "Update";
            intTravelerId = travelerId;

            
            traveler = new Traveler();
            traveler.TravelerId = travelerId;

            
            txtName.Text = row["TravelerName"].ToString();
            mtbMobile.Text = row["MobileNo"].ToString();
            mtbNID.Text = row["NID"].ToString();
            txtDeparture.Text = row["DepartureFrom"].ToString();
            nudPassangerNo.Value = Convert.ToInt32(row["NoOfPersonsToGo"]);
            dtpStartDate.Value = Convert.ToDateTime(row["TravelStartDate"]);

            if (row["TravelEndDate"] != DBNull.Value)
                dtpEndDate.Value = Convert.ToDateTime(row["TravelEndDate"]);

            chkIsRegular.Checked = row["IsRegular"] != DBNull.Value && Convert.ToBoolean(row["IsRegular"]);

            
            previousImage = row["ImageUrl"].ToString();
            string imagePath = Path.Combine(Application.StartupPath, "images", previousImage);
            string defaultPath = Path.Combine(Application.StartupPath, "images", "noimage.png");

            if (File.Exists(imagePath) && previousImage != "noimage.png")
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                using (var ms = new MemoryStream(imageBytes))
                {
                    pbUpload.Image = Image.FromStream(ms);
                }
                isDefaultImage = false;
            }
            else
            {
                byte[] imageBytes = File.ReadAllBytes(defaultPath);
                using (var ms = new MemoryStream(imageBytes))
                {
                    pbUpload.Image = Image.FromStream(ms);
                }
                isDefaultImage = true;
            }

            
            traveler.TravelPlans = ConvertDataTableToTravelPlan(travelerId);

          
            LoadTravelPlanGrid(traveler.TravelPlans);
        }


        private List<TravelPlan> ConvertDataTableToTravelPlan(int travelerId)
        {
            List<TravelPlan> list = new List<TravelPlan>();
            DataTable dt = repo.GetPlansByTravelerId(travelerId);

            foreach (DataRow row in dt.Rows)
            {
                TravelPlan plan = new TravelPlan();

                
                plan.TravelPlanId = Convert.ToInt32(row["TravelPlanId"]);

                plan.TravelerId = Convert.ToInt32(row["TravelerId"]);
                plan.TripPackageId = Convert.ToInt32(row["TripPackageId"]);
                plan.DesiredPlacesToVisit = row["DesiredPlacesToVisit"].ToString();
                plan.TravelMode = row["TravelMode"].ToString();
                plan.EstimatedHour = Convert.ToInt32(row["EstimatedHour"]);

                list.Add(plan);
            }
            return list;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            List<TravelerInfoViewModel> list = new List<TravelerInfoViewModel>();
            DataTable dt = repo.GetAllTravelerInfoForReport();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var obj = new TravelerInfoViewModel();

                    
                    obj.TravelerId = Convert.ToInt32(row["TravelerId"]);
                    obj.TravelerName = row["TravelerName"].ToString();
                    obj.MobileNo = row["MobileNo"].ToString();
                    obj.NID = row["NID"].ToString();
                    obj.IsRegular = row["IsRegular"].ToString();  
                    obj.DepartureFrom = row["DepartureFrom"].ToString();
                    obj.PassengerNo = Convert.ToInt32(row["NoOfPersonsToGo"]);
                    obj.TripStartDate = Convert.ToDateTime(row["TravelStartDate"]);                  
                    obj.TripEndDate = Convert.ToDateTime(row["TravelEndDate"]);

                    
                    obj.TripPackageId = Convert.ToInt32(row["TripPackageId"]);
                    obj.TripPackageName = row["TripPackageName"].ToString();
                    obj.BookingAmount = Convert.ToDecimal(row["BookingAmount"]);

                    obj.TravelPlanId = Convert.ToInt32(row["TravelPlanId"]);
                    obj.TouristSpots = row["DesiredPlacesToVisit"].ToString();
                    obj.TravelMode = row["TravelMode"].ToString();
                    obj.EstimatedHour = Convert.ToInt32(row["EstimatedHour"]);

                    string imgName = row["ImageUrl"].ToString();
                    obj.ImageUrl = imgName; 
                    string fullPath = Path.Combine(Application.StartupPath, "images", imgName);

                    if (File.Exists(fullPath))
                    {
                        obj.ImageBinary = File.ReadAllBytes(fullPath);
                    }
                    else
                    {
                        string noImagePath = Path.Combine(Application.StartupPath, "images", "noimage.png");
                        if (File.Exists(noImagePath))
                            obj.ImageBinary = File.ReadAllBytes(noImagePath);
                    }

                    list.Add(obj);
                }

                using (FrmReportViewers frmObj = new FrmReportViewers(list))
                {
                    frmObj.ShowDialog();
                }
            }
        }
    }
}
