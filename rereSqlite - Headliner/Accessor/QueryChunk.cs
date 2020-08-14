using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class QueryChunk {
    private readonly SqliteAccessor accessor;

    private readonly AppBehind appBehind;

    private SqliteTransaction transaction;

    private List<SqliteCommand> commands;

    public bool TransactionAlreadyBegun => accessor.TransactionAlreadyBegun;

    public QueryChunk(AppBehind a) {
        appBehind = a;
        accessor = new SqliteAccessor {DataSource = appBehind.DBFilePath, Password = appBehind.Password};
    }

    public void Open() {
        accessor.Open();
        transaction = null;
        commands = new List<SqliteCommand>();
    }

    public void Begin() {
        if (TransactionAlreadyBegun) return;
        Open();
        transaction = accessor.Begin();
    }

    public void AddCommand(string arg) {
        var qs = arg.Replace(@"\r\n", @"\n").Split(';');
        foreach (var q in qs) {
            if (string.IsNullOrEmpty(q.Trim())) continue;
            accessor.QueryString = q;
            commands.Add(accessor.CreateCommand());
        }
    }

    public void Execute() {
        commands.ForEach(ExecuteSingleCommand);
        commands.Clear();
    }

    private void ExecuteSingleCommand(SqliteCommand command) {
        if (TransactionAlreadyBegun) command.Transaction = transaction;
        accessor.Execute(command);
        if (0 < accessor.QueryResult.Count)
            appBehind.AddPage(accessor);
    }

    public void Commit() {
        transaction.Commit();
        Close();
    }

    public void Rollback() {
        transaction.Rollback();
        Close();
    }

    public void Close() {
        accessor.Close();
    }
}