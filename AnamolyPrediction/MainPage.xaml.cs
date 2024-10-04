using Syncfusion.Maui.DataGrid;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using AnamolyPrediction.ViewModel;
using AnamolyPrediction.Model;
namespace AnamolyPrediction
{
    public partial class MainPage : ContentPage
    {
        public string[] generateDataAlone;
        GridReport gridReport;
        OpenAi openAi;

        public MainPage()
        {
            openAi = new OpenAi();
            InitializeComponent();
        }

        async private void Button_Clicked(object sender, EventArgs e)
        {

            Indicator.IsRunning = true;
            gridReport = new GridReport()
            {
                DataSource = (this.dataGrid.BindingContext as MachineDataRepository)!.MachineDataCollection,
            };
            var gridReportJson = GetSerializedGridReport(gridReport);
            string userInput = ValidateAndGeneratePrompt(gridReportJson);
            var result = await openAi.GetResponseFromOpenAI(userInput);
            if (result != null)
            {
                GridReport deserializeResult = new GridReport();
                try
                {
                    result = result.Replace("```json", "").Replace("```", "").Trim();
                    deserializeResult = DeserializeResult(result);
                    string[] anamoly = deserializeResult.DataSource.Select(x => x.AnomalyDescription).ToArray();
                    generateDataAlone = deserializeResult.DataSource.Select(x => x.MachineID).ToArray();
                    ColorConverter colorConverter = new ColorConverter();
                    colorConverter.GetString(anamoly);
                    var anomalyDescriptionColumn = new DataGridTextColumn() { HeaderText = "Anomaly Description", MappingName = "AnomalyDescription", ColumnWidthMode = ColumnWidthMode.Auto };
                    int index = -1;
                    this.dataGrid.Columns.Add(anomalyDescriptionColumn);
                    foreach (var item in gridReport.DataSource)
                    {
                        if (generateDataAlone.Contains(item.MachineID))
                        {
                            index++;
                            item.AnomalyDescription = deserializeResult.DataSource[index].AnomalyDescription;
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            else
            {
                await Task.Delay(1000);
                gridReport = new GridReport()
                {
                    DataSource = (this.dataGrid.BindingContext as MachineDataRepository)!.MachineDataCollection,
                };
                int maxTemp = gridReport.DataSource.Max(t => t.Temperature);
                int maxPressure = gridReport.DataSource.Max(x => x.Pressure);
                int maxVoltage = gridReport.DataSource.Max(y => y.Voltage);
                int maxMotorSpeed = gridReport.DataSource.Max(z => z.MotorSpeed);
                string[] anamolyDescription = ["Since the mentioned temperature is too high than expected, it is marked as anomaly data", "Since the mentioned voltage is too high than expected, it is marked as anomaly data", "Since the mentioned motor speed is too high than expected, it is marked as anomaly data", "Since the mentioned pressure is too high than expected, it is marked as anomaly data"];
                foreach (var item in gridReport.DataSource)
                {
                    if (item.Temperature == maxTemp)
                    {
                        item.AnomalyDescription = anamolyDescription[0];
                    }
                    else if (item.Voltage == maxVoltage)
                    {
                        item.AnomalyDescription = anamolyDescription[1];
                    }
                    else if (item.MotorSpeed == maxMotorSpeed)
                    {
                        item.AnomalyDescription = anamolyDescription[2];
                    }
                    else if (item.Pressure == maxPressure)
                    {
                        item.AnomalyDescription = anamolyDescription[3];
                    }
                }

                ColorConverter colorConverter = new ColorConverter();
                colorConverter.GetString(anamolyDescription);
                var des = new DataGridTextColumn() { HeaderText = "AnomalyDescription", MappingName = "AnomalyDescription", MinimumWidth = 250 };

                this.dataGrid.Columns.Add(des);

            }
            Indicator.IsRunning = false;
        }

        private string GetSerializedGridReport(GridReport report)
        {
            return JsonConvert.SerializeObject(report);
        }
        private GridReport DeserializeResult(string result)
        {
            return JsonConvert.DeserializeObject<GridReport>(result)!;
        }

        private static string ValidateAndGeneratePrompt(string data)
        {
            return $"Given the following datasource are bounded in the Grid table\n\n{data}.\n Return the anomaly data rows (ie. pick the ir-relevant datas mentioned in the corresponding table) present in the table mentioned above as like in the same JSON format (it should referred as DataSource) provided do not change the format. Example: Watch out the production rate count and the factors that is used to acheive the mentioned production rate(Temprature, Pressure, Motor Speed) If the production rate is not relevant to the concern factors mark it as anomaly Data. If it is anomaly data then due to which column data it is marked as anomaly that particular column name should be updated in the AnomalyFieldName. Also Update the AnomalyDescription stating that due to which reason it is marked as anomaly a short description. Example if the data is marked as anomaly due to the Temperature column, Since the mentioned temperature is too high than expected, it is marked as anomaly data.\n\nGenerate an output in JSON format only and Should not include any additional information or contents in response";
        }
    }
    public class ColorConverter : IValueConverter
    {
        static string[] data;
        public void GetString(string[] Data)
        {
            data = Data;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo info)
        {


            var gridCell = (value as DataGridCell);
            if (gridCell != null && data != null)
            {
                var cellValue = gridCell.CellValue;
                var rowData = (gridCell.DataColumn!.RowData as MachineData)!.AnomalyDescription;

                var mappingName = gridCell.DataColumn!.DataGridColumn!.MappingName;

                if (mappingName == "AnomalyDescription")
                {
                    if (cellValue != null && data.Contains(cellValue))
                    {
                        Debug.WriteLine(mappingName);
                        return Colors.IndianRed;
                    }

                    return Colors.LightGreen;
                }

                var conditions = new Dictionary<string, string>
                {
                    { "Temperature", "temperature" },
                    { "Voltage", "voltage" },
                    { "Pressure", "pressure" },
                    { "MotorSpeed", "motor speed" }
                };

                if (conditions.TryGetValue(mappingName, out var keyword) && rowData.Contains(keyword))
                {
                    return Colors.IndianRed;
                }
            }
            return Colors.White;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
