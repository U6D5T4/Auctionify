import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-chat-item',
    templateUrl: './chat-item.component.html',
    styleUrls: ['./chat-item.component.scss'],
})
export class ChatItemComponent {
    @Input() imageUrl!: string | null;
    @Input() userName!: string;
    @Input() messageText!: string;
    @Input() messageDate!: Date;
    @Input() unreadCount: number | undefined;
    @Input() isActive!: boolean;
    @Input() email!: string;

    constructor() {}
}
