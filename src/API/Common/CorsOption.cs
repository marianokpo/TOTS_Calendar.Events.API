namespace API.Common;

public class CorsOption
{
      public List<string> Origins { get; set; }

      public List<string> Headers { get; set; }

      public List<string> ExposedHeaders { get; set; }

      public TimeSpan? MaxAge { get; set; }

      public List<string> Methods { get; set; }

      public CorsOption()
      {
            Origins = new List<string>();
            Headers = new List<string>();
            ExposedHeaders = new List<string>();
            Methods = new List<string>();
      }
}