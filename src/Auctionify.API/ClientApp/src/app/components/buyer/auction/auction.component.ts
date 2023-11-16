import { Component } from '@angular/core';

@Component({
  selector: 'app-auction',
  templateUrl: './auction.component.html',
  styleUrls: ['./auction.component.scss']
})
export class AuctionComponent {
  links: any[] = Array(8).fill({ imgUrl: 'path/to/image.jpg', linkText: 'Link' });
}
