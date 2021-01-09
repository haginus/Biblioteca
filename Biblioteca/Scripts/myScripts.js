function attachMultiSelect(container, input, options, initialOptions) {
    // Create select elements
    let selectedOptionsContainer = $(`<ul class="list-group" style="max-width: 280px;"></ul>`);
    let addNewInput = $(`<input type="text" class="form-control text-box single-line" placeholder="Add new..." style="display: inline-block;" />`);
    let autocompleteContainer = $(`<div class="autocomplete-area"></div>`);
    let addNewGroup = $(`<div class=""></div>`);
    addNewGroup.append(addNewInput);
    addNewGroup.append(autocompleteContainer);
    container.html(``);
    container.append(selectedOptionsContainer);
    container.append(addNewGroup);

    selectedOptions = [];

    const removeItem = (id, el) => {
        selectedOptions = selectedOptions.filter(opt => opt != id);
        el.remove();
        input.val(selectedOptions.join(','));
    }

    const addItem = (id) => {
        let option = options.find(option => option.id == id);
        let el = $(`<li class="list-group-item action-list"><span>${option.name}</span></li>`);
        let itemRemoveBtn = $(`<button type="button" class="btn btn-danger btn-sm">Remove</button>`);
        el.append(itemRemoveBtn);
        selectedOptionsContainer.append(el);

        selectedOptions.push(id);
        input.val(selectedOptions.join(','));

        itemRemoveBtn.click(e => removeItem(id, e.target.parentElement));
    }

    for (let i = 0; i < initialOptions.length; i++) {
        addItem(initialOptions[i]);
    }



    const showAutomplete = () => {
        autocompleteContainer.width(addNewInput.outerWidth());
        let text = addNewInput.val();
        handleKeyUp(text);
        autocompleteContainer.show();
    }

    const hideAutocomplete = () => {
        setTimeout(() => {
            autocompleteContainer.hide();
        }, 200);
    }

    const handleKeyUp = (text) => {
        text = normalizeText(text);
        autocompleteContainer.html(``);
        let res = options.filter(option => normalizeText(option.name).includes(text) && !selectedOptions.includes(option.id));
        for (let i = 0; i < res.length; i++) {
            let item = res[i];
            let el = $(`<div class="autocomplete-item">${item.name}</div>`);
            el.click(function () {
                addItem(item.id);
            });
            autocompleteContainer.append(el);
        }
    }

    addNewInput.focusin(showAutomplete);
    addNewInput.focusout(hideAutocomplete);
    addNewInput.keyup(e => handleKeyUp(e.target.value));

}

const normalizeText = (text) => {
    return text.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
}