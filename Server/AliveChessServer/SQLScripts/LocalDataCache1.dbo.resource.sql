/****
Этот сценарий SQL был создан с помощью диалогового окна "Настройка 
синхронизации данных". Сценарий содержит операторы, которые 
создают столбцы отслеживания изменений, таблицу удаленных 
элементов и триггеры базы данных сервера. Эти объекты базы данных 
необходимы службам синхронизации для успешной синхронизации 
данных между базами данных на клиенте и на сервере. Дополнительные 
сведения см. в справке "Настройка сервера базы данных для синхронизации".
****/


IF @@TRANCOUNT > 0
set ANSI_NULLS ON 
set QUOTED_IDENTIFIER ON 

GO
BEGIN TRANSACTION;


IF @@TRANCOUNT > 0
ALTER TABLE [dbo].[resource] 
ADD [LastEditDate] DateTime NULL CONSTRAINT [DF_resource_LastEditDate] DEFAULT (GETUTCDATE()) WITH VALUES
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
ALTER TABLE [dbo].[resource] 
ADD [CreationDate] DateTime NULL CONSTRAINT [DF_resource_CreationDate] DEFAULT (GETUTCDATE()) WITH VALUES
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[resource_Tombstone]')) 
BEGIN 
CREATE TABLE [dbo].[resource_Tombstone]( 
    [resource_id] Int NOT NULL,
    [DeletionDate] DateTime NULL
) ON [PRIMARY] 
END 

GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
ALTER TABLE [dbo].[resource_Tombstone] ADD CONSTRAINT [PKDEL_resource_Tombstone_resource_id]
   PRIMARY KEY CLUSTERED
    ([resource_id])
    ON [PRIMARY]
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[resource_DeletionTrigger]') AND type = 'TR') 
   DROP TRIGGER [dbo].[resource_DeletionTrigger] 

GO
CREATE TRIGGER [dbo].[resource_DeletionTrigger] 
    ON [dbo].[resource] 
    AFTER DELETE 
AS 
SET NOCOUNT ON 
UPDATE [dbo].[resource_Tombstone] 
    SET [DeletionDate] = GETUTCDATE() 
    FROM deleted 
    WHERE deleted.[resource_id] = [dbo].[resource_Tombstone].[resource_id] 
IF @@ROWCOUNT = 0 
BEGIN 
    INSERT INTO [dbo].[resource_Tombstone] 
    ([resource_id], DeletionDate)
    SELECT [resource_id], GETUTCDATE()
    FROM deleted 
END 

GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[resource_UpdateTrigger]') AND type = 'TR') 
   DROP TRIGGER [dbo].[resource_UpdateTrigger] 

GO
CREATE TRIGGER [dbo].[resource_UpdateTrigger] 
    ON [dbo].[resource] 
    AFTER UPDATE 
AS 
BEGIN 
    SET NOCOUNT ON 
    UPDATE [dbo].[resource] 
    SET [LastEditDate] = GETUTCDATE() 
    FROM inserted 
    WHERE inserted.[resource_id] = [dbo].[resource].[resource_id] 
END;
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[resource_InsertTrigger]') AND type = 'TR') 
   DROP TRIGGER [dbo].[resource_InsertTrigger] 

GO
CREATE TRIGGER [dbo].[resource_InsertTrigger] 
    ON [dbo].[resource] 
    AFTER INSERT 
AS 
BEGIN 
    SET NOCOUNT ON 
    UPDATE [dbo].[resource] 
    SET [CreationDate] = GETUTCDATE() 
    FROM inserted 
    WHERE inserted.[resource_id] = [dbo].[resource].[resource_id] 
END;
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;
COMMIT TRANSACTION;
