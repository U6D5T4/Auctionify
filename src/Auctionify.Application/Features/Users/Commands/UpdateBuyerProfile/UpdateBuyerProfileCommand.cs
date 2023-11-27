using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Users.Commands.UpdateBuyerProfile
{
	public class UpdateBuyerProfileCommand : IRequest<UpdatedBuyerProfileResponse>
	{
		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public string? PhoneNumber { get; set; }

		public string? AboutMe { get; set; }

		public IFormFile? ProfilePicture { get; set; }

		public bool DeleteProfilePicture { get; set; }
	}

	public class UpdateBuyerProfileCommandHandler
		: IRequestHandler<UpdateBuyerProfileCommand, UpdatedBuyerProfileResponse>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;
		private readonly IMapper _mapper;

		public UpdateBuyerProfileCommandHandler(
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions,
			IMapper mapper
		)
		{
			_currentUserService = currentUserService;
			_userManager = userManager;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
			_mapper = mapper;
		}

		public async Task<UpdatedBuyerProfileResponse> Handle(
			UpdateBuyerProfileCommand request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			user.FirstName = request.FirstName;
			user.LastName = request.LastName;
			user.PhoneNumber = request.PhoneNumber;
			user.AboutMe = request.AboutMe;

			if (request.ProfilePicture != null)
			{
				if (user.ProfilePicture != null)
				{
					await _blobService.DeleteFileBlobAsync(
						_azureBlobStorageOptions.UserProfilePhotosFolderName,
						user.ProfilePicture
					);
				}

				var filePath = _azureBlobStorageOptions.UserProfilePhotosFolderName;
				var guid = Guid.NewGuid().ToString();

				var fileName =
					$"{Path.GetFileNameWithoutExtension(request.ProfilePicture.FileName)}_{guid}{Path.GetExtension(request.ProfilePicture.FileName)}";

				user.ProfilePicture = fileName;

				await _blobService.UploadFileBlobAsync(request.ProfilePicture, filePath, guid);
			}
			else
			{
				if (request.DeleteProfilePicture && user.ProfilePicture != null)
				{
					await _blobService.DeleteFileBlobAsync(
						_azureBlobStorageOptions.UserProfilePhotosFolderName,
						user.ProfilePicture
					);

					user.ProfilePicture = null;
				}
			}

			await _userManager.UpdateAsync(user);

			var response = _mapper.Map<UpdatedBuyerProfileResponse>(user);

			if (user.ProfilePicture != null)
			{
				var profilePictureUrl = _blobService.GetBlobUrl(
					_azureBlobStorageOptions.UserProfilePhotosFolderName,
					user.ProfilePicture
				);

				response.ProfilePictureUrl = profilePictureUrl;
			}

			return response;
		}
	}
}
