import { Component, Inject, Input } from '@angular/core';

@Component({
    selector: 'app-chat-item',
    templateUrl: './chat-item.component.html',
    styleUrls: ['./chat-item.component.scss'],
})
export class ChatItemComponent {
    @Input() imageUrl!: string;
    @Input() userName!: string;
    @Input() messageText!: string;
    @Input() messageDate!: string;
    @Input() unreadCount: number | undefined;

    constructor() {}
}
