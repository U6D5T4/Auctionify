import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Conversation } from 'src/app/models/chats/chat-models';

@Component({
    selector: 'app-chats',
    templateUrl: './chats.component.html',
    styleUrls: ['./chats.component.scss'],
})
export class ChatsComponent {
    @Input() conversations!: Conversation[];
    @Output() conversationChosen: EventEmitter<any> = new EventEmitter();

    conversationClick(id: number) {
        this.conversationChosen.emit(id);
    }
}
