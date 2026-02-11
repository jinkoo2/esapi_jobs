using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMS.ARIA.DocumentService.WebService.WebClient;

namespace test
{
    class Program
    {
        internal static string SERVICE_URL = "https://hfhsrodb:56001/DocumentService";
        internal static string USERID = "doc_service_user";
        internal static string PASSWORD = "adminNIS";

        private static DocumentService CreateService()
        {
            return new DocumentService(SERVICE_URL, USERID, PASSWORD);
        }

        static void Main(string[] args)
        {
            // ignore SSL warning.
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (
                (sender, certificate, chain, sslPolicyErrors) => true
                    );

            try
            {
                Console.Write("Creating a service object...");
                VMS.ARIA.DocumentService.WebService.WebClient.DocumentService service = new DocumentService(SERVICE_URL, USERID, PASSWORD);
                Console.WriteLine("done.");

                // ping
                {
                    Console.Write("Ping...");
                    VMS.ARIA.REST.Shared.WebClientBase.Entities.PingResponse res = service.Ping();
                    Console.WriteLine("done;Ping=" + res.Success.ToString() + ".");
                }

                // ping with auth
                {
                    Console.Write("Ping with Authentication...");
                    VMS.ARIA.REST.Shared.WebClientBase.Entities.PingResponse res = service.PingWithAuthentication();
                    Console.WriteLine("done;Ping=" + res.Success.ToString() + ".");
                }

                // search doc
                {
                    VMS.ARIA.DocumentService.WebService.Entities.DocumentSearchParameters pm = new VMS.ARIA.DocumentService.WebService.Entities.DocumentSearchParameters();
                    {
                        //'#' followed by an ID1 value, 
                        //'$' followed by an ENM pt_id value, 
                        //'~' followed by a RadOnc PatientSer value.
                        pm.PatientId = "#12018792";
                    }
                    
                    VMS.ARIA.DocumentService.WebService.Entities.DocumentSearchResults results = service.SearchDocuments(pm);
                    foreach (VMS.ARIA.DocumentService.WebService.Entities.Document doc in results.Results)
                    {
                        //Console.Write(string.Format("PtId={0}, PtVisitId={1}, PtVisitNoteId={2}", doc.PtId, doc.PtVisitId, doc.PtVisitNoteId));
                        VMS.ARIA.DocumentService.WebService.Entities.DocumentDetailed dd = service.GetDocument("$" + doc.PtId, doc.PtVisitId, doc.PtVisitNoteId);

                        Console.Write(string.Format("DocumentType={0},AuthoredBy={1},FileFormat={2}", dd.DocumentType, dd.AuthoredBy, dd.FileFormat));

                        // save the file
                        string path = string.Format("C:\\Users\\jkim3\\Desktop\\New folder\\file.{0}.{1}.{2}.{3}",
                            dd.PtId,
                            dd.PtVisitId,
                            dd.PtVisitNoteId,
                            dd.FileFormat);

                        System.IO.File.WriteAllBytes(path, dd.BinaryContentAsBytes);

                        Console.WriteLine("");

                        // insert the doc to another patient
                        {
                            Console.Write("Inserting to PT #000000102...");
                            VMS.ARIA.DocumentService.WebService.Entities.InsertDocumentParameters pm2 = new VMS.ARIA.DocumentService.WebService.Entities.InsertDocumentParameters();
                            pm2.PatientId = "#000000102";
                            pm2.FileFormat = dd.FileFormat;
                            pm2.DateOfService = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                            pm2.DateEntered = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                            pm2.BinaryContentAsBytes = dd.BinaryContentAsBytes;
                            pm2.DocumentType = dd.DocumentType;
                            //'$' followed bya userid or a userid@instid combination, or a 
                            // '~' followed by a RadOnc/OSP UserId value
                            pm2.UserId = "~jkim3";
                            pm2.AuthoredByUserId = "~jkim3";
                            pm2.SupervisedByUserId = dd.SupervisedBy;

                            service.InsertDocument(pm2);

                            Console.WriteLine("");
                        }

                    }
                }



            }
            catch (Exception exn)
            {
                Console.WriteLine(exn.ToString());
            }


        }
    }
}