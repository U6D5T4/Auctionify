export interface ChatItem {
    imageUrl: string;
    userName: string;
    messageText: string;
    unread: string | undefined;
}

export interface ConversationsResponse {
    user: ChatUser;
    conversations: Conversation[];
}

export interface Conversation {
    id: number;
    lotId: number;
    chatUser: ChatUser;
    lastMessage: ChatMessage;
    unreadMessagesCount: number;
    isActive: boolean;
}

export interface ChatUser {
    id: number;
    fullName: string;
    profilePhoto: string | null;
    role: string;
    email: string;
}

export interface ChatMessage {
    id: number;
    senderId: number;
    conversationId: number;
    body: string;
    timeStamp: Date;
    isRead: boolean;
}
