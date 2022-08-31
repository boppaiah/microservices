using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DisocuntService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DisocuntService> _logger;
        private readonly IMapper _mapper;

        public DisocuntService(IDiscountRepository repository, ILogger<DisocuntService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Disocunt for product name:{request.ProductName} not found!! "));

            _logger.LogInformation($"Coverting enity to coupon model");
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;

        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.CreateDiscount(coupon);

            var couponModel = _mapper.Map<CouponModel>(coupon);
            _logger.LogInformation($"Disocunt created for product {request.Coupon.ProductName} with ID: {couponModel.Id}");
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var isDeleted = await _repository.DeleteDiscount(request.Coupon.ProductName);
            if (isDeleted)
                throw new RpcException(new Status(StatusCode.NotFound, $"Disocunt for product name:{request.Coupon.ProductName} cannot be deleted!! "));
            var deleteDiscountResponse = new DeleteDiscountResponse
            {
                Success = isDeleted
            };

            return deleteDiscountResponse;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.UpdateDiscount(coupon);

            var couponModel = _mapper.Map<CouponModel>(coupon);
            _logger.LogInformation($"Disocunt updated for product {request.Coupon.ProductName} with ID: {couponModel.Id}");
            return couponModel;
        }
    }
}
