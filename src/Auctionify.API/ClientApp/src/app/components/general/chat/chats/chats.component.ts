import {
    Component,
    EventEmitter,
    Input,
    OnChanges,
    OnInit,
    Output,
    SimpleChanges,
} from '@angular/core';
import { Conversation } from 'src/app/models/chats/chat-models';

@Component({
    selector: 'app-chats',
    templateUrl: './chats.component.html',
    styleUrls: ['./chats.component.scss'],
})
export class ChatsComponent implements OnChanges {
    @Input() conversations!: Conversation[];
    private chosenId: number = 0;
    @Output() conversationChosen: EventEmitter<any> = new EventEmitter();

    ngOnChanges(changes: SimpleChanges): void {
        if (this.chosenId > 0) {
            console.log(this.chosenId);
            this.handleConversationChange(this.chosenId);
        }
    }

    conversationClick(id: number) {
        this.conversationChosen.emit(id);
        this.handleConversationChange(id);
    }

    handleConversationChange(id: number) {
        this.conversations = this.conversations.map((v) => {
            v.isActive = false;
            if (v.id == id) {
                v.isActive = true;
            }
            return v;
        });
        this.chosenId = id;
    }
}
