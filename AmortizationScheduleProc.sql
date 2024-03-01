CREATE PROCEDURE AmortizationScheduleProc
AS
BEGIN
DECLARE @LoanAmount DECIMAL(10, 2) = 36000.00;
DECLARE @InitialRate DECIMAL(5, 2) = 0.08
DECLARE @RecycleRate DECIMAL(5, 2) = 0.045;
DECLARE @TotalPayments INT = 36;
DECLARE @RecyclePayments INT = 48;
DECLARE @Start INT = 1;
DECLARE @RemainingAmount DECIMAL(10, 2) = @LoanAmount;

DECLARE @R decimal(10, 4) =  @InitialRate / 12
DECLARE @TotalRate decimal(10, 4) = 1 + (@TotalPayments*@R)/2 + (POWER(@TotalPayments*@R,2))/12;
DECLARE @TotalAmount decimal(10, 4) =  @LoanAmount * @TotalRate;
DECLARE @MontlyAmount decimal =  @TotalAmount / 36;

select @TotalAmount

CREATE TABLE #AmortizationSchedule (
    PaymentNumber INT,
    PaymentAmount DECIMAL(10, 2),
    PrincipalPayment DECIMAL(10, 2),
    Rate DECIMAL(10, 2),
    RemainingAmount DECIMAL(10, 2)
);

CREATE TABLE #RecycleAmortizationSchedule (
    PaymentNumber INT,
    PaymentAmount DECIMAL(10, 2),
    PrincipalPayment DECIMAL(10, 2),
    Rate DECIMAL(10, 2),
    RemainingAmount DECIMAL(10, 2)
);

WHILE @Start <= @TotalPayments
BEGIN
    DECLARE @Rate DECIMAL(10, 2);
    DECLARE @Principal DECIMAL(10, 2);
    DECLARE @Payment DECIMAL(10, 2);

	Set @Rate = @R * @RemainingAmount
    SET @Principal = @MontlyAmount - @Rate;
    SET @Payment = @MontlyAmount;
    SET @RemainingAmount = @RemainingAmount - @Principal;

    INSERT INTO #AmortizationSchedule (
        PaymentNumber,
        PaymentAmount,
        PrincipalPayment,
        Rate,
        RemainingAmount
    ) VALUES (
        @Start,
        @Payment,
        @Principal,
        @Rate,
        @RemainingAmount
    );
    SET @Start = @Start + 1;
END;

SELECT * FROM #AmortizationSchedule;
----- recycle calculating -------
DECLARE @RemainingAmountForRecycle float
select  @RemainingAmountForRecycle = RemainingAmount 
FROM #AmortizationSchedule
where paymentNumber = 12;

set @R =  @RecycleRate / 12
set @TotalRate  =  1 + (@RecyclePayments*@R)/2 + (POWER(@RecyclePayments*@R,2))/12;
set @TotalAmount =  @RemainingAmountForRecycle * @TotalRate;
set @MontlyAmount =  @TotalAmount / 48;
set @start = 1;
set @RemainingAmount = @RemainingAmountForRecycle
--- recycle loop-----
WHILE @Start <= @RecyclePayments
BEGIN
	Set @Rate = @R * @RemainingAmount
    SET @Principal = @MontlyAmount - @Rate;
    SET @Payment = @MontlyAmount;
    SET @RemainingAmount = @RemainingAmount - @Principal;

    INSERT INTO #RecycleAmortizationSchedule (
        PaymentNumber,
        PaymentAmount,
        PrincipalPayment,
        Rate,
        RemainingAmount
    ) VALUES (
        @Start,
        @Payment,
        @Principal,
        @Rate,
        @RemainingAmount
    );
    SET @Start = @Start + 1;
END;


SELECT * FROM #RecycleAmortizationSchedule;
DROP TABLE #AmortizationSchedule;
DROP TABLE #RecycleAmortizationSchedule;
END


