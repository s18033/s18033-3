CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT 
AS 
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN;

	DECLARE @IdStudy INT = (SELECT IdStudy FROM dbo.Studies WHERE Name=@Studies)
	IF @IdStudy IS NULL
	BEGIN
		RAISERROR('No studies have been found', 16, 16);
		RETURN;
	END

	DECLARE @CurrentIdEnrollment INT = (SELECT IdEnrollment FROM dbo.Enrollment WHERE IdStudy=@IdStudy AND Semester=@Semester)
	
	DECLARE @NextIdEnrollment INT = (SELECT IdEnrollment FROM dbo.Enrollment WHERE IdStudy=@IdStudy AND Semester=(@Semester+1))
	IF @NextIdEnrollment IS NULL
	BEGIN
		SET @NextIdEnrollment = (SELECT COUNT(*) FROM dbo.Enrollment);
		INSERT INTO dbo.Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@NextIdEnrollment + 1, @Semester + 1, @IdStudy, '2024-01-01');
		UPDATE dbo.Student SET IdEnrollment=@NextIdEnrollment WHERE IdEnrollment = @CurrentIdEnrollment;
	END
	ELSE
	BEGIN
		UPDATE dbo.Student SET IdEnrollment=@NextIdEnrollment WHERE IdEnrollment = @CurrentIdEnrollment;
	END;

	COMMIT;

	RETURN @NextIdEnrollment;
END;