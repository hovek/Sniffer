USE [msdb]

/****** Object:  Job [Drop BadOrGoodFilter unused procedures]    Script Date: 17.6.2012. 18:02:18 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 17.6.2012. 18:02:18 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'Drop BadOrGoodFilter unused procedures', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'Daria-PC\Hrvoje Batrnek', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Drop procedures]    Script Date: 17.6.2012. 18:02:18 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Drop procedures', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'DECLARE	@DateLastExecuted DATETIME
SET @DateLastExecuted = DATEADD(DAY, -14, GETDATE())

DECLARE	@tblProcedures TABLE ( NAME VARCHAR(200) )
INSERT	INTO @tblProcedures
		SELECT	name
		FROM	sys.objects o
		INNER JOIN sys.dm_exec_procedure_stats deps ON o.object_id = deps.object_id
		WHERE	o.type = ''P'' AND deps.last_execution_time < @DateLastExecuted AND o.name LIKE ''spBadOrGoodFilter%''
        
WHILE ( EXISTS ( SELECT	*
				 FROM	@tblProcedures ) ) 
	BEGIN 
		DECLARE	@Name VARCHAR(200)
		SET @Name = NULL
		SELECT TOP 1
				@Name = Name
		FROM	@tblProcedures

		DECLARE	@ExecSQL NVARCHAR(500)
		SELECT	@ExecSQL = ''drop procedure '' + @Name
		EXECUTE sp_executesql @ExecSQL

		DELETE	FROM @tblProcedures
		WHERE	Name = @Name
	END', 
		@database_name=N'TipElder', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Every day', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20120617, 
		@active_end_date=99991231, 
		@active_start_time=210000, 
		@active_end_time=235959, 
		@schedule_uid=N'31eea143-bef6-4125-8e2b-866ed65d0b30'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave: