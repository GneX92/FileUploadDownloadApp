using System.Diagnostics;
using FileUploadDownload.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FileUploadDownload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private NetdbContext _ctx = new();

        public HomeController( ILogger<HomeController> logger )
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var table = _ctx.DocStore.ToList();

            return View( table );
        }

        [HttpPost]
        public IActionResult UploadFile( IEnumerable<IFormFile> allFiles )
        {
            string folderpath = Path.Combine( Directory.GetCurrentDirectory() , "UploadedFiles" );

            if ( !Directory.Exists( folderpath ) )
                Directory.CreateDirectory( folderpath );

            if ( allFiles != null )
                foreach ( var file in allFiles )
                {
                    if ( file.Length > 0 )
                    {
                        string filePath = Path.Combine( folderpath , Path.GetFileName( file.FileName ) );

                        using ( var memoryStream = new MemoryStream() )
                        {
                            file.CopyTo( memoryStream );

                            _ctx.DocStore.Add( new DocStore
                            {
                                DocName = file.FileName ,
                                DocData = memoryStream.ToArray() ,
                                ContentLength = file.Length / 1000 ,
                                InsertionDate = DateTime.Now ,
                                ContentType = file.ContentType
                            } );
                        }
                    }
                }

            _ctx.SaveChanges();

            var newTable = _ctx.DocStore.ToList();

            return PartialView( "_FileList" , newTable );
        }

        public IActionResult FileDelete( int id )
        {
            // Not Finished missing AJAX functionality

            var fileDoc = _ctx.DocStore.Where( f => f.DocId == id ).FirstOrDefault();

            _ctx.DocStore.Remove( fileDoc );

            _ctx.SaveChanges();

            var newTable = _ctx.DocStore.ToList();

            return PartialView( "_FileList" , newTable );
        }

        public FileResult FileDownload( int id )
        {
            var fileDoc = _ctx.DocStore.Where( f => f.DocId == id ).FirstOrDefault();

            return File( fileDoc.DocData , fileDoc.ContentType , fileDoc.DocName );
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache( Duration = 0 , Location = ResponseCacheLocation.None , NoStore = true )]
        public IActionResult Error()
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }
    }
}