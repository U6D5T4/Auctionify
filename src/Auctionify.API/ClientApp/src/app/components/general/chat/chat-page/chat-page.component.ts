import {
    Component,
    ElementRef,
    OnInit,
    ViewChild,
    effect,
} from '@angular/core';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { Conversation } from 'src/app/models/chats/chat-models';
import { Client } from 'src/app/web-api-client';
import { ChatsComponent } from '../chats/chats.component';

@Component({
    selector: 'app-chat-page',
    templateUrl: './chat-page.component.html',
    styleUrls: ['./chat-page.component.scss'],
})
export class ChatPageComponent implements OnInit {
    isUserBuyer: boolean = false;

    constructor(private client: Client, private authService: AuthorizeService) {
        effect(() => {
            this.isUserBuyer = this.authService.isUserBuyer();
        });
    }
    ngOnInit(): void {
        this.getAllUserConversations();
    }
    chosenConversation: Conversation | null = null;
    chatConversations: Conversation[] = [];

    getAllUserConversations() {
        this.client.getAllUserConversations().subscribe({
            next: (result) => {
                this.chatConversations = result.conversations;
            },
        });
    }

    handleConversationChange(id: number) {
        this.chosenConversation = null;
        this.chosenConversation = this.chatConversations.find(
            (x) => x.id == id
        )!;
    }

    getMainChatContainerWidthClass() {
        if (this.isUserBuyer) {
            return 'chat-width-buyer';
        } else {
            return 'chat-width-seller';
        }
    }
}
