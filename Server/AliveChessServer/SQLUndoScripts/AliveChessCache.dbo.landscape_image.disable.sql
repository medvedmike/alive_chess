IF EXISTS (SELECT * FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID(N'[dbo].[landscape_image]')) 
   ALTER TABLE [dbo].[landscape_image] 
   DISABLE  CHANGE_TRACKING
GO
