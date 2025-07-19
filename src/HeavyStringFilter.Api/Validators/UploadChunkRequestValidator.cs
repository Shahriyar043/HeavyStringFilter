using FluentValidation;
using HeavyStringFilter.Api.Models;

namespace HeavyStringFilter.Api.Validators;

public class UploadChunkRequestValidator : AbstractValidator<UploadChunkRequest>
{
    public UploadChunkRequestValidator()
    {
        RuleFor(x => x.UploadId)
            .NotEmpty().WithMessage("UploadId must not be empty");

        RuleFor(x => x.ChunkIndex)
            .GreaterThanOrEqualTo(0).WithMessage("ChunkIndex must be non-negative");

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data must not be empty")
            .MaximumLength(104_857_600).WithMessage("Chunk data is too large (max 100MB)");
    }
}
