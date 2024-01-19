import { Component, OnInit, effect } from '@angular/core';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { Conversation } from 'src/app/models/chats/chat-models';
import { Client } from 'src/app/web-api-client';

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
        this.client.getAllUserConversations().subscribe({
            next: (result) => {
                this.chatConversations = result.conversations;
            },
        });
    }
    chosenConversation: Conversation | null = null;
    chatConversations: Conversation[] = [];

    handleConversationChange(id: number) {
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
