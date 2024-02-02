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
import { SignalRService } from 'src/app/services/signalr-service/signalr.service';
import { Subject } from 'rxjs';

@Component({
    selector: 'app-chat-page',
    templateUrl: './chat-page.component.html',
    styleUrls: ['./chat-page.component.scss'],
})
export class ChatPageComponent implements OnInit {
    isUserBuyer: boolean = false;
    private isSignalrConnected = false;
    updateChatSubject: Subject<void> = new Subject<void>();
    currentUserId: number = 0;

    constructor(
        private client: Client,
        private authService: AuthorizeService,
        private signalRService: SignalRService
    ) {
        effect(() => {
            this.isUserBuyer = this.authService.isUserBuyer();
        });
        effect(() => {
            this.currentUserId = this.authService.getUserId()!;
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

                if (!this.isSignalrConnected) {
                    this.chatConversations.forEach(async (element) => {
                        await this.signalRService.joinConversationGroup(
                            element.id
                        );

                        this.signalRService.onReceiveChatMessage((senderId) => {
                            if (senderId != this.currentUserId)
                                this.getAllUserConversations();
                            this.chatConversations = this.chatConversations.map(
                                (v) => {
                                    v.isActive = false;
                                    if (v.id == this.chosenConversation?.id) {
                                        v.isActive = true;
                                    }
                                    return v;
                                }
                            );
                            if (this.chosenConversation?.id == element.id)
                                this.updateChatSubject.next();
                        }, element.id);

                        this.isSignalrConnected = true;
                    });
                }
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
