using Content.Api.DTOs;
using Content.Api.DTOs.Responses;

namespace Content.Api.Services.Interfaces;

public interface IContentCmsService
{
    // About Pages
    Task<ApiResponse<List<AboutPageResponse>>> GetAllAboutPagesAsync();
    Task<ApiResponse<AboutPageResponse>> GetAboutPageBySlugAsync(string slug);
    Task<ApiResponse<AboutPageResponse>> UpdateAboutPageAsync(string slug, UpdateAboutPageRequest request, Guid? updatedBy = null);

    // Partners
    Task<ApiResponse<List<PartnerResponse>>> GetAllPartnersAsync(bool includeInactive = false);
    Task<ApiResponse<PartnerResponse>> CreatePartnerAsync(CreatePartnerRequest request);
    Task<ApiResponse<PartnerResponse>> UpdatePartnerAsync(int id, UpdatePartnerRequest request);
    Task<ApiResponse<bool>> DeletePartnerAsync(int id);

    // Gallery
    Task<ApiResponse<List<GalleryResponse>>> GetAllGalleriesAsync(int? programmeId = null);
    Task<ApiResponse<GalleryResponse>> CreateGalleryItemAsync(CreateGalleryRequest request);
    Task<ApiResponse<bool>> DeleteGalleryItemAsync(int id);

    // Invitations
    Task<ApiResponse<InvitationResponse>> SendFriendInvitationAsync(SendInvitationRequest request, Guid? senderUserId = null);
}
