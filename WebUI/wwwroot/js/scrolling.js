export function scrollContainerBy(element, amount) {
    if (!element) {
        console.warn("Element is null");
        return;
    }
    element.scrollBy({ left: amount, behavior: 'smooth' });
}