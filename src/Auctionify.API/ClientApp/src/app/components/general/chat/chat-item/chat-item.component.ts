import { formatDate } from '@angular/common';
import { Component, Inject, Input, LOCALE_ID } from '@angular/core';

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

    constructor(@Inject(LOCALE_ID) public locale: string) {}

    formatDate(date: Date | null): string {
        return date ? formatDate(date, 'hh:mm aa', this.locale) : '';
    }
}
