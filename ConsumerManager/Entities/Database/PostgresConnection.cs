namespace ConsumerManager.Entities.Database
{
  public record PostgresConnection
  {
    public string Hostname { get; set; }

    public string Database { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public PostgresConnection()
    {
      Database = string.Empty;
      Hostname = string.Empty;
      Username = string.Empty;
      Password = string.Empty;
    }
    public override string ToString()
    {
      return $"Host={Hostname};Database={Database};Username={Username};Password={Password}";
    }
  }
}
