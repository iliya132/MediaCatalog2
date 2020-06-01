using MediaCatalog2.Model.DTO;
using MediaCatalog2.Model.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace MediaCatalog2.Model.Implementations
{
    public class SQLiteDataProvider : IProgramsProvider
    {
        readonly SqliteConnection _connection;

        public SQLiteDataProvider()
        {
            _connection = new SqliteConnection("DataSource=MediaCatalog.db");
            _connection.Open();
            EnsureCreated();
        }

        #region EnsureCreated
        private void EnsureCreated()
        {
            EnsureProgramTableCreated();
            EnsureMediaFilesTableCreated();
        }

        private void EnsureProgramTableCreated()
        {
            SqliteCommand command = new SqliteCommand(
                "CREATE TABLE IF NOT EXISTS 'TVProgram' (" +
                "'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "'Name' TEXT NOT NULL," +
                "'Description' TEXT," +
                "'Actors' TEXT," +
                "'YearEstablished' INTEGER NOT NULL," +
                "'AvatarSourcePath' TEXT" +
                ");", _connection);
            command.ExecuteNonQuery();
        }

        private void EnsureMediaFilesTableCreated()
        {
            SqliteCommand command = new SqliteCommand(
                "CREATE TABLE IF NOT EXISTS 'MediaFile' (" +
                "'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'Name' TEXT NOT NULL, " +
                "'CompleteName' TEXT NOT NULL, " +
                "'Format' TEXT NOT NULL, " +
                "'FrameSize' TEXT NOT NULL, " +
                "'TimingInFrames' INTEGER NOT NULL, " +
                "'Program_Id' INTEGER NOT NULL, " +
                "FOREIGN KEY('Program_Id') REFERENCES 'Programs'('Id')" +
                ");", _connection);
            command.ExecuteNonQuery();
        }
        #endregion

        public IEnumerable<TV_ProgramDTO> GetPrograms()
        {
            List<TV_ProgramDTO> programsToReturn = new List<TV_ProgramDTO>();
            SqliteCommand command = new SqliteCommand(
                "select TVProgram.Id as 'TVProgramId', TVProgram.name as 'ProgramName', " +
                "TVProgram.Description, TVProgram.Actors, TVProgram.YearEstablished, TVProgram.AvatarSourcePath, " +
                "MediaFile.Id as 'MediaFileId', MediaFile.name as 'MediaFileName', MediaFile.CompleteName, " +
                "MediaFile.Format, MediaFile.FrameSize, MediaFile.TimingInFrames, MediaFile.Program_Id " +
                "from TVProgram left join MediaFile on MediaFile.Program_Id = TVProgram.Id order by TVProgram.Id", _connection);
            SqliteDataReader reader = command.ExecuteReader(); ;
            int PreviousProgramId = -1;
            TV_ProgramDTO newProgram = new TV_ProgramDTO();

            while (reader.Read())
            {
                if (!reader.HasRows)
                {
                    break;
                }

                // Загружаем программы. 
                // Т.к. используется left join, то возможны ситуации когда одна и та же программа содержит 2 видео. 
                // Что бы избежать дублирования введена проверка (запоминаем предыдущий Id и сравниваем с текущим)
                // 1Program 1Media <--Программу добавляем
                // 1Program 2Media <--Программу игнорируем
                // Принимается что -1 это первый запуск
                // При этом учитывается что произведена сортировка по восходящей по полю Id - "order by TVProgram.Id"
                int currentProgramId = int.Parse(reader["TVProgramId"].ToString());
                if (PreviousProgramId != currentProgramId || PreviousProgramId == -1)
                {
                    newProgram = new TV_ProgramDTO();
                    newProgram.Id = int.Parse(reader["TVProgramId"].ToString());
                    newProgram.Name = reader["ProgramName"].ToString();
                    newProgram.Description = reader["Description"].ToString();
                    newProgram.Actors = reader["Actors"].ToString();
                    newProgram.AvatarSourcePath = reader["AvatarSourcePath"].ToString();
                    PreviousProgramId = currentProgramId;
                    int.TryParse(reader["YearEstablished"].ToString(), out int tempYear);
                    newProgram.YearEstablished = tempYear;
                    programsToReturn.Add(newProgram);
                }

                //Добавляем зависимые видеозаписи
                if (!string.IsNullOrEmpty(reader["MediaFileId"].ToString()))
                {
                    MediaFileDTO newMediaFile = new MediaFileDTO();
                    newMediaFile.Id = int.Parse(reader["MediaFileId"].ToString());
                    newMediaFile.Name = reader["MediaFileName"].ToString();
                    newMediaFile.CompleteName = reader["CompleteName"].ToString();
                    newMediaFile.Format = reader["Format"].ToString();
                    newMediaFile.FrameSize = reader["FrameSize"].ToString();
                    int.TryParse(reader["TimingInFrames"].ToString(), out int tempFrames);
                    newMediaFile.TimingInFrames = tempFrames;
                    newMediaFile.ParentTvProgramId = int.Parse(reader["Program_Id"].ToString());
                    newProgram.MediaFiles.Add(newMediaFile);
                }
            }
            return programsToReturn;
        }

        public void AddProgram(TV_ProgramDTO Program)
        {
            if (!IsProgramDataValid(Program)) { throw new FormatException("Поступило некорректное значение"); }

            string sql = string.Format(
                "insert into TVProgram('Name', 'Description', 'Actors', 'YearEstablished', 'AvatarSourcePath') " +
                "values ('{0}', '{1}', '{2}', {3}, '{4}'); SELECT last_insert_rowid();",
                Program.Name, Program.Description, Program.Actors, Program.YearEstablished, Program.AvatarSourcePath);
            SqliteCommand command = new SqliteCommand(sql, _connection);
            int lastInsertedRowId = int.Parse(command.ExecuteScalar().ToString());
            Program.Id = lastInsertedRowId;
        }

        private bool IsProgramDataValid(TV_ProgramDTO program)
        {
            return true; //В рамках ТЗ не требуется, но в проекте я бы реализовал
        }

        public async void AddVideoFileAsync(MediaFileDTO video)
        {
            if (!IsMediaDataValid(video)) { throw new FormatException("Поступило некорректное значение"); }

            string sql = string.Format(
                "insert into MediaFile('Name', 'CompleteName', 'Format', 'FrameSize', TimingInFrames, Program_Id) " +
                "values ('{0}', '{1}', '{2}', '{3}', {4}, '{5}'); SELECT last_insert_rowid();",
                video.Name, video.CompleteName, video.Format, video.FrameSize, video.TimingInFrames, video.ParentTvProgramId);
            SqliteCommand command = new SqliteCommand(sql, _connection);
            object lastInsertedRowId = await command.ExecuteScalarAsync();
            video.Id = Convert.ToInt32(lastInsertedRowId);
        }

        private bool IsMediaDataValid(MediaFileDTO video)
        {
            return true; //В рамках ТЗ не требуется, но в проекте я бы реализовал
        }

        public void RemoveMediaFile(MediaFileDTO video)
        {
            string sql = string.Format("Delete from MediaFile where MediaFile.Id = {0}", video.Id);
            SqliteCommand command = new SqliteCommand(sql, _connection);
            command.ExecuteNonQuery();
        }

        public void Remove(TV_ProgramDTO Program)
        {
            string sql = string.Format("Delete from TVProgram where TVProgram.Id = {0}", Program.Id);
            SqliteCommand command = new SqliteCommand(sql, _connection);
            command.ExecuteNonQuery();
        }

        public async void CommitChangesAsync()
        {
            //Метод создан для работы с EntityFramework. При работе напрямую с SQL в этой функции нет надобности.
        }

        public void EditProgram(TV_ProgramDTO oldValue, TV_ProgramDTO newValue)
        {
            if (!IsProgramDataValid(newValue)) { throw new FormatException("Поступило некорректное значение"); }
            string sql = string.Format("update TVProgram SET name='{0}', Description='{1}', Actors='{2}', YearEstablished={3}, AvatarSourcePath='{4}' where id={5}",
                newValue.Name, newValue.Description, newValue.Actors, newValue.YearEstablished, newValue.AvatarSourcePath, oldValue.Id);
            SqliteCommand command = new SqliteCommand(sql, _connection);
            command.ExecuteNonQuery();
        }
    }
}
