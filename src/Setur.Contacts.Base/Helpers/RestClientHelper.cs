using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Setur.Contacts.Base.Helpers;

public enum HttpVerb
{
    Get,
    Post,
    Put,
    Delete
}

/// <summary>
/// Bu sınıf Proje içerisinde herhangi bir dış servise REST API ile bağlanmak için kullanılmaktadır.<br/>
/// İçerisinde Get ve Post işlemleri için gerekli olan metotlar bulunmaktadır.<br/>
/// </summary>
public class RestClientHelper
{
    /// <summary>
    /// Servis URL'si
    /// </summary>
    public string EndPoint { get; set; }
    /// <summary>
    /// Servise gönderilecek olan JSON formatında string data
    /// </summary>
    public string PostData { get; set; } = default!;
    /// <summary>
    /// Gerekli ise Bearer formatına uygun olarak gönderilecek olan JWT token
    /// </summary>
    public string Token { get; set; } = default!;

    ///// <summary>
    ///// HTTP Metodu
    ///// </summary>
    //public HttpVerb HttpMethod { get; set; }

    public RestClientHelper()
    {
        EndPoint = "";
        //HttpMethod = HttpVerb.Get;
    }

    public string Get()
    {

        string strResponseValue = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EndPoint);

        request.Method = HttpVerb.Get.ToString();
        HttpWebResponse response = null;
        request.ContentType = "application/json";

        if (!string.IsNullOrWhiteSpace(Token))
            request.Headers.Set("Authorization", $"Bearer {Token}");

        try
        {
            response = (HttpWebResponse)request.GetResponse();
            using var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                using var reader = new StreamReader(responseStream);
                strResponseValue = reader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            strResponseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
        }
        finally
        {
            ((IDisposable)response)?.Dispose();
        }

        return strResponseValue;
    }

    public bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    public string Post()
    {
        var strResponseValue = string.Empty;

        var request = (HttpWebRequest)WebRequest.Create(EndPoint);

        request.Method = HttpVerb.Post.ToString();
        HttpWebResponse response = null;
        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

        var byteArray = Encoding.UTF8.GetBytes(PostData);
        request.ContentType = "application/json";
        if (!string.IsNullOrEmpty(Token))
            request.Headers.Set("Authorization", $"Bearer {Token}");
        request.ContentLength = byteArray.Length;


        var dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();

        try
        {
            response = (HttpWebResponse)request.GetResponse();
            using Stream responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                using StreamReader reader = new StreamReader(responseStream);
                strResponseValue = reader.ReadToEnd();
            }
        }
        catch (WebException ew)
        {
            using var str = ew.Response.GetResponseStream();
            using var reader = new StreamReader(str);
            strResponseValue = reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            strResponseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
        }
        finally
        {
            ((IDisposable)response)?.Dispose();
        }

        return strResponseValue;
    }
}