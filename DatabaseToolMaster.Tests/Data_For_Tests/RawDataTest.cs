namespace DatabaseToolMaster.Tests.Data_For_Tests;

public class RawDataTest
{
    public static string GetRawDataExample()
    {
       // find the file in the project
       // get current fullpath
       var currentFullPath = Directory.GetCurrentDirectory();
       
       // get up 3 directories
       currentFullPath += @"/../../..";
       
       // get the file
       var filePath = currentFullPath + @"/Data_For_Tests/mysqlsampledatabase.sql";
       
       // check if the file exists
         if (File.Exists(filePath))
         {
              // read the file
              var filer = File.ReadAllText(filePath);
              
              //check if the file is there
              if (filer != null)
              {
                return filer;
              }
         }
         else
         {
             // get files in the directory
                var files = Directory.GetFiles(currentFullPath);
                var hghggh = files;
         }
       
       // read the file
       var file = File.ReadAllText(currentFullPath + @"\Data_For_Tests\RawDataExample.txt");
       
       //check if the file is there
         if (file != null)
         {
              return file;
         }
       


       return "";
    }
}