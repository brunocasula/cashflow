using AutoMapper;
using CashFlow.Common.Enums;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Exception.ExceptionsBase;
//using CashFlow.Infrastructure.DataAccess;

namespace CashFlow.Application.UseCases.Expenses.Register;
public class RegisterExpenseUseCase : IRegisterExpenseUseCase    
{
    private readonly IExpensesWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterExpenseUseCase(
        IExpensesWriteOnlyRepository repository, 
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseRegisterExpenseJson> Execute(RequestExpenseJson request)
    {
        Validate(request);

        //var dbContext = new CashFlowDbContext();
        //var entity = new Expense
        //{
        //    Amount = request.Amount,
        //    Date = request.Date,
        //    Description = request.Description,
        //    Title = request.Title,
        //    PaymentType = (PaymentType)request.PaymentType,
        //};

        var entity = _mapper.Map<Expense>(request);

        await _repository.Add(entity);
        await _unitOfWork.Commit();

        //dbContext.Expenses.Add(entity);
        //dbContext.SaveChanges();

        //return new ResponseRegisterExpenseJson();
        return _mapper.Map<ResponseRegisterExpenseJson>(entity);
    }

    private void Validate(RequestExpenseJson request)
    {
        var validator = new ExpenseValidator();
        
        var result = validator.Validate(request);

        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(erro => erro.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
