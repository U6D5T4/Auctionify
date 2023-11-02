using Auctionify.Application.Common.Interfaces.Repositories;
using FluentValidation;

namespace Auctionify.Application.Features.Lots.Commands.DeleteLotFile
{
	public class DeleteLotFileValidator : AbstractValidator<DeleteLotFileCommand>
	{
		private readonly IFileRepository _fileRepository;

		public DeleteLotFileValidator(IFileRepository fileRepository)
		{
			_fileRepository = fileRepository;

			RuleFor(x => x.LotId)
				.MustAsync(
					async (id, cancellationToken) =>
					{
						var file = await _fileRepository.GetAsync(
							predicate: x => x.LotId == id,
							cancellationToken: cancellationToken
						);

						return file != null;
					}
				)
				.WithMessage("Lot with this Id does not exist");
		}
	}
}
