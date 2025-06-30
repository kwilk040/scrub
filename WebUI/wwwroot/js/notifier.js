export function pushMessage(message) {
    const notifier = document.getElementById('notifier');
    if (!notifier) {
        throw new Error("notifier not found.")
    }
    const node = createMessageContainer(message.message, message.type);
    notifier.appendChild(node);

    setTimeout(() => {
        notifier.removeChild(node);
    }, 5000)

    function createMessageContainer(messageText, type) {
        if (type != 'success' && type != 'error') {
            throw new Error(`invalid message type,: ${type}`)
        }

        const node = document.createElement('div')
        node.classList.add('notifier-message')
        node.classList.add('rounded')
        node.classList.add(type)

        const paragraph = document.createElement('p')
        paragraph.innerText = messageText
        node.appendChild(paragraph)
        return node;
    }
}