IF NOT EXISTS (SELECT * FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID(N'[dbo].[conversation_context]')) 
   ALTER TABLE [dbo].[conversation_context] 
   ENABLE  CHANGE_TRACKING
GO
