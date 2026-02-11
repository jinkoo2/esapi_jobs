using System;
using VMS.ARIA.DocumentService.WebService.Entities;
using VMS.ARIA.DocumentService.WebService.WebClient;
using VMS.ARIA.REST.Shared.WebClientBase;

namespace VarianDocumentService.Samples
{
    static class Sample
    {
        internal static string SERVICE_URL = "https://hfhsrodb:56001/DocumentService";
        internal static string USERID = "doc_service_user";
        internal static string PASSWORD = "adminNIS";

        private static DocumentService CreateService()
        {
            // Causes this application to trust all SSL certificates. The Document Service is installed with a self-signed
            // server certificate by default which means that, from a client application's perspective, the certificate was not
            // issued by a trusted certificate authority. For most applications running within a LAN this is not a big concern  
            // and it is recommended that in this case server certificate validation be ignored. Please see the User Guide for 
            // alternative options, such as adding the server as a trusted certification authority or installing the client
            // certificate onto client machines.
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (
                (sender, certificate, chain, sslPolicyErrors) => true
            ); 

            // Creates a web client for the document service:
            return new DocumentService(SERVICE_URL, USERID, PASSWORD); 
        }

        public static Document InsertDocumentTest(byte[] documentContents, string fileFormat)
        {
            Document document = null;

            var service = CreateService();

            string patientId1Value = "#td123"; // Use # for an id1 value, $ for a varianenm database pt_id value, or ~ for a variansystem database PatientSer value.

            try
            {
                document = service.InsertDocument(new InsertDocumentParameters()
                {
                    PatientId = patientId1Value,
                    BinaryContentAsBytes = documentContents,
                    FileFormat = fileFormat,
                    DateOfService = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")

                });
            }
            catch (WebClientBaseException ex)
            {
                Console.WriteLine(ex.Message);

                // Example of handling a known service error:
                if (ex.ServiceError != null && ex.ServiceError.ErrorCode == "ECB_000")
                {
                    Console.WriteLine("The login credentials are not correct. Please check them and try again.");
                }

                throw;
            }

            return document;
        }

        public static DocumentDetailed GetDocumentTest()
        {
            DocumentDetailed document = null;

            var service = CreateService();

            string patientId1Value = "#td123"; // Use # for an id1 value, $ for a varianenm database pt_id value, or ~ for a variansystem database PatientSer value.
            string patientVisitIdValue = "1"; // Part of the document's unique identifier
            string visitNoteId = "1"; // Part of the document's unique identifier

            try
            {
                document = service.GetDocument(patientId1Value, patientVisitIdValue, visitNoteId);
            }
            catch (WebClientBaseException ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }

            return document;
        }

        public static Document[] GetDocumentsTest()
        {
            Document[] documents = null;

            var service = CreateService();

            string patientId1Value = "#td123"; // Use # for an id1 value, $ for a varianenm database pt_id value, or ~ for a variansystem database PatientSer value.

            try
            {
                documents = service.GetDocuments(patientId1Value);
            }
            catch (WebClientBaseException ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }

            return documents;
        }

        public static DocumentSearchResults SearchTest()
        {
            DocumentSearchResults results = null;

            var service = CreateService();

            string userIdValue = "~SysAdmin"; // Use $ for a varianenm user_id value, or ~ for a Platform Portal (OSP) user id

            try
            {
                results = service.SearchDocuments(new DocumentSearchParameters()
                {
                    EnteredByUserId = userIdValue
                });
            }
            catch (WebClientBaseException ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }

            return results;
        }
    }
}
