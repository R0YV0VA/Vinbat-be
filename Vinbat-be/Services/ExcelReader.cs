using System.Data.OleDb;
using System.Data;
using Vinbat_be.Models;

namespace Vinbat_be.Services;

public class ExcelReader
{
    public async Task<List<Battery>> ReadBattariesFromExcel(string sheetName, string path)
    {
        var excelData = ReadExcelFile(sheetName, path);
        if (!ValidateExcelColumnsBatteries(excelData))
        {
            File.Delete(path);
            return null;
        }
        var batteries = new List<Battery>();
        foreach (DataRow row in excelData.Rows)
        {
            var battery = new Battery
            {
                Id = new Random().Next(1000, Int32.MaxValue),
                Name = row[0].ToString(),
                Brand = row[1].ToString(),
                Capacity = Convert.ToInt32(row[2]),
                CapacitiveGroup = Convert.ToInt32(row[3]),
                StartingСurrent = Convert.ToInt32(row[4]),
                Voltage = Convert.ToInt32(row[5]),
                PositiveTerminal = Convert.ToInt32(row[6]),
                Application = Convert.ToInt32(row[7]),
                TypeOfElectolite = Convert.ToInt32(row[8]),
                Image = row[9].ToString(),
                Description = row[10].ToString(),
                Price = Convert.ToInt32(row[11]),
                WholesalePrice = Convert.ToInt32(row[12]),
                Availability = Convert.ToBoolean(row[13])
            };
            batteries.Add(battery);
        }
        File.Delete(path);
        return batteries;
    }

    public async Task<List<Tires>> ReadTiresFromExcel(string sheetName, string path)
    {
        var excelData = ReadExcelFile(sheetName, path);
        if (!ValidateExcelColumnsTires(excelData))
        {
            File.Delete(path);
            return null;
        }
        var tires = new List<Tires>();
        foreach (DataRow row in excelData.Rows)
        {
            var tire = new Tires
            {
                Id = new Random().Next(1000, Int32.MaxValue),
                Name = row[0].ToString(),
                Season = Convert.ToInt32(row[1]),
                Radius = Convert.ToInt32(row[2]),
                Size = row[3].ToString(),
                Brand = row[4].ToString(),
                Country = row[5].ToString(),
                Date = row[6].ToString(),
                Description = row[7].ToString(),
                Image = row[8].ToString(),
                Price = Convert.ToInt32(row[9]),
                WholesalePrice = Convert.ToInt32(row[10]),
                Availability = Convert.ToBoolean(row[11])
            };
            tires.Add(tire);
        }
        File.Delete(path);
        return tires;
    }
    private DataTable ReadExcelFile(string sheetName, string path)
    {
        using (OleDbConnection conn = new OleDbConnection())
        {
            DataTable dt = new DataTable();
            string Import_FileName = path;
            string fileExtension = Path.GetExtension(Import_FileName);
            if (fileExtension == ".xls")
                conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 8.0;HDR=YES;'";
            if (fileExtension == ".xlsx")
                conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            using (OleDbCommand comm = new OleDbCommand())
            {
                //command to fetch rows from Upload sheet
                comm.CommandText = "Select * from [" + sheetName + "$]";
                comm.Connection = conn;
                using (OleDbDataAdapter da = new OleDbDataAdapter())
                {
                    da.SelectCommand = comm;
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }

    private bool ValidateExcelColumnsBatteries(DataTable dt)
    {
        var columns = dt.Columns;
        var expectedColumns = new List<string>
        {
            "Name",
            "Brand",
            "Capacity",
            "CapacitiveGroup",
            "StartingСurrent",
            "Voltage",
            "PositiveTerminal",
            "Application",
            "TypeOfElectolite",
            "Image",
            "Description",
            "Price",
            "WholesalePrice",
            "Availability"
        };
        if (columns.Count != 14)
            return false;
        for(int i = 0; i < columns.Count; i++)
        {
            if (!expectedColumns.Contains(columns[i].Caption))
            {
                return false;
            }
        }
        return true;
    }
    private bool ValidateExcelColumnsTires(DataTable dt)
    {
        var columns = dt.Columns;
        var expectedColumns = new List<string>
        {
            "Name",
            "Season",
            "Radius",
            "Size",
            "Brand",
            "Country",
            "Date",
            "Description",
            "Image",
            "Price",
            "WholesalePrice",
            "Availability"
        };
        if (columns.Count != 12)
            return false;
        for (int i = 0; i < columns.Count; i++)
        {
            if (!expectedColumns.Contains(columns[i].Caption))
            {
                return false;
            }
        }
        return true;
    }
}
