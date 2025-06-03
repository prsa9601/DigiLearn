using DigiLearn.WebApi.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionModule.Domain;
using TransactionModule.Services;
using TransactionModule.Services.DTOs.Commands;
using TransactionModule.Services.DTOs.Queries;

namespace DigiLearn.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ApiController
    {
        private readonly IUserTransactionService _service;

        public TransactionController(IUserTransactionService service)
        {
            _service = service;
        }
        #region Commands
        [HttpPost("CreateTransaction")]
        public async Task<ApiResult<Guid>> CreateTransaction(CreateTransactionCommand command)
        {
            try
            {
                Guid transactionId = await _service.CreateTransaction(command);
                return CommandResult<Guid>(new Common.Application.OperationResult<Guid>
                {
                    Message = "عملیات با موفقیت انجام شد.",
                    Status = Common.Application.OperationResultStatus.Success,
                    Title = "عملیات موفق",
                    Data = transactionId
                });

            }
            catch (Exception ex)
            {
                return CommandResult<Guid>(new Common.Application.OperationResult<Guid>
                {
                    Message = "خطایی در عملیات رخ داده است!",
                    Status = Common.Application.OperationResultStatus.Error,
                    Title = "عملیات ناموفق"
                });
            }
        }
        [HttpPost("PaymentSuccess")]
        public async Task<ApiResult> PaymentSuccess(TransactionPaymentSuccessCommand command)
        {
            try
            {
                await _service.PaymentSuccess(command);
                return CommandResult(new Common.Application.OperationResult
                {
                    Message = "عملیات با موفقیت انجام شد.",
                    Status = Common.Application.OperationResultStatus.Success,
                    Title = "عملیات موفق"
                });

            }
            catch (Exception ex)
            {
                return CommandResult(new Common.Application.OperationResult
                {
                    Message = "خطایی در عملیات رخ داده است!",
                    Status = Common.Application.OperationResultStatus.Error,
                    Title = "عملیات ناموفق"
                });
            }
        }
        [HttpPost("PaymentError")]
        public async Task<ApiResult> PaymentError(TransactionPaymentErrorCommand command)
        {
            try
            {
                await _service.PaymentError(command);
                return CommandResult(new Common.Application.OperationResult
                {
                    Message = "عملیات با موفقیت انجام شد.",
                    Status = Common.Application.OperationResultStatus.Success,
                    Title = "عملیات موفق"
                });

            }
            catch (Exception ex)
            {
                return CommandResult(new Common.Application.OperationResult
                {
                    Message = "خطایی در عملیات رخ داده است!",
                    Status = Common.Application.OperationResultStatus.Error,
                    Title = "عملیات ناموفق"
                });
            }
        }
        #endregion

        #region Queries

        
        /// <example>
        /// نمونه ریکوئست  => api/Transaction/GetTransactionById?{transactionId}
        /// </example>
        [HttpGet("GetUserTransaction")]
        public async Task<ApiResult<UserTransaction>> GetTransactionById(Guid transactionId)
        {
            return QueryResult(await _service.GetTransactionById(transactionId));
        }

        [HttpGet("GetTransactionsByFilter")]
        public async Task<ApiResult<UserTransactionFilterDto>> GetTransactionsByFilter
            ([FromQuery] UserTransactionFilterParams queryParams)
        {
            return QueryResult(await _service.GetTransactionsByFilter(queryParams));
        }

        [HttpGet("GetCancelTransactionsCount")]
        public async Task<ApiResult<int>> GetCancelTransactionsCount
            ([FromQuery] string startDate, string endDate,
            TransactionFor transactionFor, TransactionStatus status)
        {
            return QueryResult(await _service.GetCancelTransactionsCount(startDate,
                endDate, transactionFor, status));
        }

        #endregion
    }
}
