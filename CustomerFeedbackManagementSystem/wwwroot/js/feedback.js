

$(document).ready(async function () {

    $.fn.dataTableExt.oApi.fnStandingRedraw = function (oSettings) {
        if (oSettings.oFeatures.bServerSide === false) {
            var before = oSettings._iDisplayStart;
            oSettings.oApi._fnReDraw(oSettings);
            oSettings._iDisplayStart = before;
            oSettings.oApi._fnCalculateEnd(oSettings);
        }
        oSettings.oApi._fnDraw(oSettings);
    };

    const userIdElement = document.querySelector("#UserId");
    const userId = userIdElement ? userIdElement.innerHTML : null;

    const createFeedbackButton = document.querySelector("#CreateFeedbackButton");
    const feedbackDescription = document.querySelector("#FeedbackDescription");
    const feedbackFormErrors = document.querySelector("#FeedbackFormErrors");
    const editButtonsContainer = document.querySelector("#EditButtonsContainer");
    const editButton = document.querySelector("#EditButton");
    const cancelEditButton = document.querySelector("#CancelEditButton");


    const $categoriesFilterSelect = $("#CategoryFilter");
    const $timeframeFilterSelect = $("#TimeframeFilter");
    const $categoriesSelect = $("#Category");
    const $feedbackReceiverSelect = $("#FeedbackReceiverID");

    $.getJSON("/Category/GetCategories", function (data) {
        var options = '';
        $.each(data, function (index, item) {
            options += "<option value='" + item.categoryId + "'>" + item.categoryName + "</option>";
        });

        $categoriesFilterSelect.html('<option value="all">All</option>' + options);
        $categoriesSelect.html('<option value="">Select Category</option>' + options);
    });

    $.getJSON("/FeedbackReceiver/GetFeedbackReceivers", function (data) {
        var options = '<option value="">Select Feedback Receiver</option>';
        $.each(data, function (index, item) {
            options += "<option value='" + item.feedbackReceiverId + "'>" + item.feedbackReceiverName + "</option>";
        });

        $feedbackReceiverSelect.html(options);
    });

    const tableAjaxUrl = "/Feedbacks/LoadData";
    const columns = [
        { "data": "feedbackReceiverName", "name": "FeedbackReceiverName", "autoWidth": true },
        { "data": "userName", "name": "UserName", "autoWidth": true },
        { "data": "category", "name": "Category", "autoWidth": true },
        { "data": "description", "name": "Description", "autoWidth": true },
        {
            "data": "submissionDate",
            "name": "SubmissionDate",
            "autoWidth": true,
            "render": function (data, type, row) {
                return row.submissionDateString;
            }
        },
    ];

    if (userId) {
        columns.push(
            {
                "render": function (data, type, row) {
                    if (row.userId !== userId) {
                        return '';
                    }

                    return '<button type="button" class="edit-feedback btn btn-info" data-feedback="' + encodeURIComponent(JSON.stringify(row)) + '">Edit</button>' + '<button type="button" class="delete-feedback btn btn-danger" data-feedback-id="' + row.feedbackId + '">Delete</button>';
                }
            }
        );
    }

    const $feedbackTable = $("#feedback-table").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "orderMulti": false,
        "ajax": {
            "url": tableAjaxUrl,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": columns

    });


    $categoriesFilterSelect.add($timeframeFilterSelect).on('change', function () {
        const params = new URLSearchParams();
        params.append("category", $categoriesFilterSelect.val());
        params.append("timeframe", $timeframeFilterSelect.val());
        console.log(tableAjaxUrl + "?" + params.toString());
        $feedbackTable.ajax.url(tableAjaxUrl + "?" + params.toString());
        $feedbackTable.ajax.reload();
    });

    document.addEventListener('click', function (e) {

        if (e.target.classList.contains('edit-feedback')) {
            createFeedbackButton.classList.toggle('d-none', true);
            editButtonsContainer.classList.toggle('d-none', false);
            editButton.dataset.feedback = e.target.dataset.feedback;

            const feedbackToEdit = JSON.parse(decodeURIComponent(e.target.dataset.feedback));

            $categoriesSelect.val(feedbackToEdit.categoryId);
            $feedbackReceiverSelect.val(feedbackToEdit.feedbackReceiverID);
            feedbackDescription.value = feedbackToEdit.description;
            window.scrollTo(0, document.body.scrollHeight);
            e.preventDefault();
        }

        if (e.target.classList.contains('delete-feedback')) {
            e.preventDefault();
            Swal.fire({
                title: 'Warning',
                text: 'Do you want to delete this feedback?',
                icon: 'warning',
                confirmButtonText: 'Yes',
                showCancelButton: true,
                showLoaderOnConfirm: true,
                preConfirm: async () => {
                    try {
                        const response = await fetch('/Feedbacks/Delete/' + e.target.dataset.feedbackId, {
                            method: 'POST',
                            headers: {
                                'RequestVerificationToken': $(".AntiForge" + " input").val()
                            }
                        })

                        if (!response.ok) {
                            return Swal.showValidationMessage(`
                      ${JSON.stringify(await response.json())}
                    `);
                        }
                    } catch (error) {
                        Swal.showValidationMessage(`
                    Request failed: ${error}
                  `);
                    }
                },
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.close();
                }

                $feedbackTable.ajax.reload();
            });

        }
    });

    const validateFeedbackForm = () => {
        const categoryVal = $categoriesSelect.val();
        const feedbackReceiverVal = $feedbackReceiverSelect.val();
        const descriptionVal = feedbackDescription.value;

        $categoriesSelect[0].classList.toggle('is-invalid', false);
        $feedbackReceiverSelect[0].classList.toggle('is-invalid', false);
        feedbackDescription.classList.toggle('is-invalid', false);
        feedbackFormErrors.classList.toggle('d-none', true);


        const errors = [];

        const parsedCategoryVal = parseInt(categoryVal, 10);
        const categoryValIsInt = (!isNaN(parsedCategoryVal) && categoryVal === '' + parsedCategoryVal)

        const parsedFeedbackReceiverVal = parseInt(feedbackReceiverVal, 10);
        const parsedFeedbackReceiverValIsInt = (!isNaN(parsedFeedbackReceiverVal) && feedbackReceiverVal === '' + parsedFeedbackReceiverVal)


        if (!categoryValIsInt) {

            $categoriesSelect[0].classList.toggle('is-invalid', true);
            errors.push('Please select a valid Category');
        }

        if (!parsedFeedbackReceiverValIsInt) {

            $feedbackReceiverSelect[0].classList.toggle('is-invalid', true);
            errors.push('Please select a valid Feedback Receiver');
        }

        if (!descriptionVal) {
            feedbackDescription.classList.toggle('is-invalid', true);
            errors.push('Please add a Description to the feedback');
        }

        const hasErrors = errors.length != 0;

        if (hasErrors) {
            let errorItems = '';

            for (var i = 0; i < errors.length; i++) {
                errorItems += '<li>' + errors[i] + '</li>';
            }

            feedbackFormErrors.innerHTML = '<ul>' + errorItems + '</ul>';
            feedbackFormErrors.classList.toggle('d-none', false);
        }

        return {
            hasErrors: hasErrors,
            data: {
                categoryId: $categoriesSelect.val(),
                feedbackReceiverId: $feedbackReceiverSelect.val(),
                description: feedbackDescription.value
            }
        }
    }

    const resetfeedbackForm = function () {
        $categoriesSelect.val("");
        $feedbackReceiverSelect.val("");
        feedbackDescription.value = "";
    }

    if (cancelEditButton) {

        cancelEditButton.addEventListener('click', function () {
            resetfeedbackForm();
            editButton.dataset.feedback = '';
            editButtonsContainer.classList.toggle('d-none', true);
            createFeedbackButton.classList.toggle('d-none', false);
        })
    }

    if (editButton) {
        editButton.addEventListener('click', async function (e) {
            e.preventDefault();
            const validationResult = validateFeedbackForm();

            if (validationResult.hasErrors) {
                return;
            }

            Swal.fire({
                title: "Updating Feedback...",
                didOpen: () => {
                    Swal.showLoading();
                },
            });

            const feedbackToUpdateOld = JSON.parse(decodeURIComponent(editButton.dataset.feedback));
            const feedbackToUpdate = validationResult.data;

            feedbackToUpdate.feedbackId = feedbackToUpdateOld.feedbackId;

            try {
                const response = await fetch('/Feedbacks/Edit', {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': $(".AntiForge" + " input").val(),
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(feedbackToUpdate)
                })


                if (response.status != 201) {
                    Swal.hideLoading();
                    return Swal.showValidationMessage(`
                      ${JSON.stringify(await response.json())}
                    `);
                }
            } catch (error) {
                Swal.hideLoading();
                Swal.showValidationMessage(`
                    Request failed: ${error}
                  `);
            }

            cancelEditButton.click();
            $feedbackTable.ajax.reload();
            Swal.close();
        });
    }

    if (createFeedbackButton) {
        createFeedbackButton.addEventListener('click', async function (e) {
            e.preventDefault();
            const validationResult = validateFeedbackForm();

            if (validationResult.hasErrors) {
                return;
            }

            Swal.fire({
                title: "Sending Feedback...",
                didOpen: () => {
                    Swal.showLoading();
                },
            });

            try {
                const response = await fetch('/Feedbacks/Create', {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': $(".AntiForge" + " input").val(),
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(validationResult.data)
                })


                if (response.status != 201) {
                    Swal.hideLoading();
                    return Swal.showValidationMessage(`
                      ${JSON.stringify(await response.json())}
                    `);
                }
            } catch (error) {
                Swal.hideLoading();
                Swal.showValidationMessage(`
                    Request failed: ${error}
                  `);
            }

            resetfeedbackForm();
            $feedbackTable.ajax.reload();
            Swal.close();
        });
    }
});