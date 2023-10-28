namespace Auctionify.Application.Features.Lots.Commands.DeleteLotFile
{
	public class DeletedLotFileResponse
	{
		public int LotId { get; set; }

		public bool WasDeleted { get; set; }
	}
}
