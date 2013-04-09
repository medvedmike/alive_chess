/****
Внимание!
 Для предотвращения потенциальной потери данных, необходимо 
проверить этот сценарий перед его выполнением.

Этот SQL-сценарий был создан с помощью диалога 
"Настройка синхронизации данных".  Этот сценарий дополняет сценарий, который может использоваться для создания
требуемых объектов базы данных для отслеживания сделанных изменений.  Этот
сценарий содержит операторы, которые удалят эти изменения.

Дополнительные сведения см. в справке "Настройка сервера базы данных для синхронизации".
****/


IF @@TRANCOUNT > 0
set ANSI_NULLS ON 
set QUOTED_IDENTIFIER ON 

GO
BEGIN TRANSACTION;


IF @@TRANCOUNT > 0
ALTER TABLE [dbo].[map_sector_image] DROP CONSTRAINT [DF_map_sector_image_LastEditDate]
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
ALTER TABLE [dbo].[map_sector_image] DROP COLUMN [LastEditDate]
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
ALTER TABLE [dbo].[map_sector_image] DROP CONSTRAINT [DF_map_sector_image_CreationDate]
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
ALTER TABLE [dbo].[map_sector_image] DROP COLUMN [CreationDate]
GO
IF @@ERROR <> 0 
     ROLLBACK TRANSACTION;


IF @@TRANCOUNT > 0
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[map_sector_image_Tombstone]') and TYPE = N'U') 
   DROP TABLE [dbo].[map_sector_image_Tombstone];


IF @@TRANCOUNT > 0
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[map_sector_image_DeletionTrigger]') AND type = 'TR') 
   DROP TRIGGER [dbo].[map_sector_image_DeletionTrigger] 

GO


IF @@TRANCOUNT > 0
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[map_sector_image_UpdateTrigger]') AND type = 'TR') 
   DROP TRIGGER [dbo].[map_sector_image_UpdateTrigger] 

GO


IF @@TRANCOUNT > 0
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[map_sector_image_InsertTrigger]') AND type = 'TR') 
   DROP TRIGGER [dbo].[map_sector_image_InsertTrigger] 

GO
COMMIT TRANSACTION;
