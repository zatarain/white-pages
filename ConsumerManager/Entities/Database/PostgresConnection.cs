namespace ConsumerManager.Entities.Database
{
  public class PostgresConnection
  {
    public string Hostname { get; set; }

    public string Database { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string ToString()
    {
      return $"Host={Hostname};Database={Database};Username={Username};Password={Password}";
    }
  }
}
