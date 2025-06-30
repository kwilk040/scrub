export function openAside(wrapper) {
    if (!wrapper) {
        console.warn("Wrapper is null");
        return;
    }
    wrapper.classList.add("open");
}

export function closeAside(wrapper) {
    if (!wrapper) {
        console.warn("Wrapper is null");
        return;
    }
    wrapper.classList.remove("open");
}
